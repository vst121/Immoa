namespace Immoa.Training;

public partial class ModelTrainingService
{
    public static void TrainRegression()
    {
        var mlContext = new MLContext();
        
        var data = DataManager.LoadAllData();
        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Categorical
            .OneHotEncoding(
                $"{nameof(ApartmentRegressionData.Regio1)}Encoded", nameof(ApartmentRegressionData.Regio1))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ApartmentRegressionData.Regio2)}Encoded", nameof(ApartmentRegressionData.Regio2)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ApartmentRegressionData.Regio3)}Encoded", nameof(ApartmentRegressionData.Regio3)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ApartmentRegressionData.LivingSpace)}Normalized", nameof(ApartmentRegressionData.LivingSpace)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ApartmentRegressionData.NoRooms)}Normalized", nameof(ApartmentRegressionData.NoRooms)))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentRegressionData.NewlyConst)}Converted", nameof(ApartmentRegressionData.NewlyConst), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentRegressionData.Balcony)}Converted", nameof(ApartmentRegressionData.Balcony), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentRegressionData.HasKitchen)}Converted", nameof(ApartmentRegressionData.HasKitchen), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentRegressionData.Cellar)}Converted", nameof(ApartmentRegressionData.Cellar), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentRegressionData.Garden)}Converted", nameof(ApartmentRegressionData.Garden), outputKind: Microsoft.ML.Data.DataKind.Single))

            .Append(mlContext.Transforms.Concatenate("Features",
                $"{nameof(ApartmentRegressionData.Regio1)}Encoded",
                $"{nameof(ApartmentRegressionData.Regio2)}Encoded",
                $"{nameof(ApartmentRegressionData.Regio3)}Encoded",
                $"{nameof(ApartmentRegressionData.LivingSpace)}Normalized",
                $"{nameof(ApartmentRegressionData.NoRooms)}Normalized",
                $"{nameof(ApartmentRegressionData.NewlyConst)}Converted",
                $"{nameof(ApartmentRegressionData.Balcony)}Converted",
                $"{nameof(ApartmentRegressionData.HasKitchen)}Converted",
                $"{nameof(ApartmentRegressionData.Cellar)}Converted",
                $"{nameof(ApartmentRegressionData.Garden)}Converted"
            ))

            .Append(mlContext.Transforms.CopyColumns("Label", nameof(ApartmentRegressionData.BaseRent)))
            .Append(mlContext.Regression.Trainers.FastTree());

        var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        var model = pipeline.Fit(trainTestData.TrainSet);

        mlContext.Model.Save(model, dataView.Schema, "ImmoaRegressionModel.zip");

        var predicts = model.Transform(trainTestData.TestSet);
        var metrics = mlContext.Regression.Evaluate(predicts);

        Console.WriteLine($"Model for Regression has been created!");
        Console.WriteLine($"RSquared: {metrics.RSquared:0.##}");
        Console.WriteLine($"Mean Absolute Error:{metrics.MeanAbsoluteError:n0}");
        Console.WriteLine($"Mean Squared Error: {metrics.MeanSquaredError:n0}");
        Console.WriteLine($"LossFunction: {metrics.LossFunction:n0}");
        Console.WriteLine($"Root Mean Squared Error(RMSE): {metrics.RootMeanSquaredError:n0}");
    }
}