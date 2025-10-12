using Tensorflow.Contexts;

namespace Immoa.Training;

public class ModelTrainer
{
    public void Train()
    {
        var mlContext = new MLContext();

        var data = LoadDataFromSql();
        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Categorical
            .OneHotEncoding("Regio1Encoded", "Regio1")
            .Append(mlContext.Transforms.NormalizeMinMax("BaseRentNormalized", nameof(ImmoItemData.BaseRent)))
            .Append(mlContext.Transforms.Conversion
                .ConvertType("GeoPlzConverted", nameof(ImmoItemData.GeoPlz), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType("LiftConverted", nameof(ImmoItemData.Lift), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Concatenate("Features", "BaseRentNormalized", "GeoPlzConverted"))
            .Append(mlContext.Transforms.CopyColumns("Label", nameof(ImmoItemData.BaseRent)))
            .Append(mlContext.Regression.Trainers.FastTree());

        var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        var model = pipeline.Fit(dataView);

        mlContext.Model.Save(model, dataView.Schema, "ImmoModel.zip");

        var predicts = model.Transform(trainTestData.TestSet);
        var metrics = mlContext.Regression.Evaluate(predicts);

        Console.WriteLine($"Model has been created!");
        Console.WriteLine($"R^2: {metrics.RSquared:0.##}");
        Console.WriteLine($"Mean Absolute Error(MAE) :{metrics.MeanAbsoluteError.ToString("n0")} Euro");
        Console.WriteLine($"Mean Squared Error(MSE) : {metrics.MeanSquaredError.ToString("n0")} Euro");
        Console.WriteLine($"Root Mean Squared Error(RMSE): {metrics.RootMeanSquaredError.ToString("n0")} Euro");
    }

    private IEnumerable<ImmoItemData> LoadDataFromSql()
    {
        var list = new List<ImmoItemData>();
        var connectionString = "Server=.;Database=ImmoDB;Integrated Security=True;TrustServerCertificate=True;";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = new SqlCommand(@"SELECT  
              [scoutId]
              , [regio1]
              , [regio2]
              , [regio3]
              , [streetPlain]
              , [houseNumber]
              , [geo_plz]
              , [serviceCharge]
              , [heatingType]
              , [newlyConst]
              , [yearConstructed]
              , [yearConstructedRange]
              , [lastRefurbish]
              , [livingSpace]
              , [livingSpaceRange]
              , [balcony]
              , [pricetrend]
              , [baseRent]
              , [baseRentRange]
              , [totalRent]
              , [noParkSpaces]
              , [firingTypes]
              , [hasKitchen]
              , [cellar]
              , [petsAllowed]
              , [lift]
              , [typeOfFlat]
              , [noRooms]
              , [noRoomsRange]
              , [floor]
              , [numberOfFloors]
              , [garden]
              , [thermalChar]
              , [heatingCosts]
              , [energyEfficiencyClass]
              , [electricityBasePrice]
              , [condition]
              FROM ImmoItems", connection);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {

            //for (int i = 0; i < reader.FieldCount; i++)
            //{
            //    if (reader.IsDBNull(i))
            //    {
            //        Console.WriteLine($"ColumnName: {reader.GetName(i)}");
            //    }
            //}

            list.Add(new ImmoItemData
            {
                ScoutId = reader.GetInt32(0),
                Regio1 = reader.GetString(1),
                Regio2 = reader.GetString(2),
                Regio3 = reader.GetString(3),
                StreetPlain = reader.GetString(4),
                HouseNumber = reader.GetString(5),
                GeoPlz = reader.GetInt32(6),
                ServiceCharge = reader.IsDBNull(7) ? -1.0F : (float)reader.GetDouble(7),
                HeatingType = reader.GetString(8),
                NewlyConst = reader.GetBoolean(9),
                YearConstructed = reader.GetString(10),
                YearConstructedRange = reader.GetString(11),
                LastRefurbish = reader.GetString(12),
                LivingSpace = (float)reader.GetDouble(13),
                LivingSpaceRange = reader.GetInt32(14),
                Balcony = reader.GetBoolean(15),
                Pricetrend = reader.IsDBNull(16) ? -1.0F : (float)reader.GetDouble(16),
                BaseRent = (float)reader.GetDouble(17),
                BaseRentRange = reader.GetInt32(18),
                TotalRent = reader.IsDBNull(19) ? -1.0F : (float)reader.GetDouble(19),
                NoParkSpaces = reader.GetString(20),
                FiringTypes = reader.GetString(21),
                HasKitchen = reader.GetBoolean(22),
                Cellar = reader.GetBoolean(23),
                PetsAllowed = reader.GetString(24),
                Lift = reader.GetBoolean(25),
                TypeOfFlat = reader.GetString(26),
                NoRooms = (float)reader.GetDouble(27),
                NoRoomsRange = reader.GetInt32(28),
                Floor = reader.GetString(29),
                NumberOfFloors = reader.GetString(30),
                Garden = reader.GetBoolean(31),
                ThermalChar = reader.IsDBNull(32) ? -1.0F : (float)reader.GetDouble(32),
                HeatingCosts = reader.GetString(33),
                EnergyEfficiencyClass = reader.GetString(34),
                ElectricityBasePrice = reader.GetString(35),
                Condition = reader.GetString(36),
            });
        }
        return list;
    }
}