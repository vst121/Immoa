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
                $"{nameof(ImmoItemData.Regio1)}Encoded", nameof(ImmoItemData.Regio1)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ImmoItemData.Regio2)}Encoded", nameof(ImmoItemData.Regio2)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ImmoItemData.Regio3)}Encoded", nameof(ImmoItemData.Regio3)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ImmoItemData.LivingSpace)}Normalized", nameof(ImmoItemData.LivingSpace)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ImmoItemData.NoRooms)}Normalized", nameof(ImmoItemData.NoRooms)))
            .Append(mlContext.Transforms.Concatenate("Features",
                $"{nameof(ImmoItemData.Regio1)}Encoded",
                $"{nameof(ImmoItemData.Regio2)}Encoded",
                $"{nameof(ImmoItemData.Regio3)}Encoded",
                $"{nameof(ImmoItemData.LivingSpace)}Normalized",
                $"{nameof(ImmoItemData.NoRooms)}Normalized"
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