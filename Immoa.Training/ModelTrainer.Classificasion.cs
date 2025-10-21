namespace Immoa.Training;

public partial class ModelTrainingService
{
    public static void TrainClassification()
    {
        var mlContext = new MLContext();

        var data = DataManager.LoadAllData()
            .Select(a => 
            {
                var rentPerM2 = a.BaseRent / a.LivingSpace;

                var category = rentPerM2 switch
                {
                    < 10 => "Budget",
                    < 20 => "Standard",
                    _ => "Luxury"
                };

                return new ApartmentClassificationData
                {
                    Regio1 = a.Regio1,
                    Regio2 = a.Regio2,
                    Regio3 = a.Regio3,
                    LivingSpace = a.LivingSpace,
                    NoRooms = a.NoRooms,
                    BaseRent = a.BaseRent,
                    Category = category
                };
            });

    var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ApartmentClassificationData.Category))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ApartmentClassificationData.Regio1)}Encoded", nameof(ApartmentClassificationData.Regio1)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ApartmentClassificationData.Regio2)}Encoded", nameof(ApartmentClassificationData.Regio2)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ApartmentClassificationData.Regio3)}Encoded", nameof(ApartmentClassificationData.Regio3)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ApartmentClassificationData.LivingSpace)}Normalized", nameof(ApartmentClassificationData.LivingSpace)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ApartmentClassificationData.NoRooms)}Normalized", nameof(ApartmentClassificationData.NoRooms)))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentClassificationData.NewlyConst)}Converted", nameof(ApartmentClassificationData.NewlyConst), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentClassificationData.Balcony)}Converted", nameof(ApartmentClassificationData.Balcony), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentClassificationData.HasKitchen)}Converted", nameof(ApartmentClassificationData.HasKitchen), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentClassificationData.Cellar)}Converted", nameof(ApartmentClassificationData.Cellar), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ApartmentClassificationData.Garden)}Converted", nameof(ApartmentClassificationData.Garden), outputKind: Microsoft.ML.Data.DataKind.Single))

            .Append(mlContext.Transforms.Concatenate("Features",
                $"{nameof(ApartmentClassificationData.Regio1)}Encoded",
                $"{nameof(ApartmentClassificationData.Regio2)}Encoded",
                $"{nameof(ApartmentClassificationData.Regio3)}Encoded",
                $"{nameof(ApartmentClassificationData.LivingSpace)}Normalized",
                $"{nameof(ApartmentClassificationData.NoRooms)}Normalized",
                $"{nameof(ApartmentClassificationData.NewlyConst)}Converted",
                $"{nameof(ApartmentClassificationData.Balcony)}Converted",
                $"{nameof(ApartmentClassificationData.HasKitchen)}Converted",
                $"{nameof(ApartmentClassificationData.Cellar)}Converted",
                $"{nameof(ApartmentClassificationData.Garden)}Converted"
            ))
            //.Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(mlContext.MulticlassClassification.Trainers.LightGbm("Label", "Features"))
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        var model = pipeline.Fit(trainTestData.TrainSet);

        mlContext.Model.Save(model, dataView.Schema, "ImmoaClassificationModel.zip");

        var predicts = model.Transform(trainTestData.TestSet);
        var metrics = mlContext.MulticlassClassification.Evaluate(predicts);

        Console.WriteLine($"Model for Classification has been created!");
        Console.WriteLine($"LogLoss: {metrics.LogLoss:0.##}");
        Console.WriteLine($"Log Loss Reduction:{metrics.LogLossReduction:0.##}");
        Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy:0.##}");
        Console.WriteLine($"Micro Accuracy: {metrics.MicroAccuracy:0.##}");
        Console.WriteLine("Confusion Matrix:");
        Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());
        for (int i = 0; i < metrics.PerClassLogLoss.Count; i++)
        {
            Console.WriteLine($"Class {i} LogLoss: {metrics.PerClassLogLoss[i]:n2}");
        }
    }
}