using Tensorflow.Contexts;

namespace Immoa.Training;

public class ModelTrainer
{
    public void Train()
    {
        var mlContext = new MLContext();
        
        var data = (new DataManager()).LoadAllData();
        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Categorical
            .OneHotEncoding(
                $"{nameof(ImmoItemData.Regio1)}Encoded", nameof(ImmoItemData.Regio1))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ImmoItemData.Regio2)}Encoded", nameof(ImmoItemData.Regio2)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                $"{nameof(ImmoItemData.Regio3)}Encoded", nameof(ImmoItemData.Regio3)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ImmoItemData.LivingSpace)}Normalized", nameof(ImmoItemData.LivingSpace)))
            .Append(mlContext.Transforms.NormalizeMinMax($"{nameof(ImmoItemData.NoRooms)}Normalized", nameof(ImmoItemData.NoRooms)))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ImmoItemData.NewlyConst)}Converted", nameof(ImmoItemData.NewlyConst), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ImmoItemData.Balcony)}Converted", nameof(ImmoItemData.Balcony), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ImmoItemData.HasKitchen)}Converted", nameof(ImmoItemData.HasKitchen), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ImmoItemData.Cellar)}Converted", nameof(ImmoItemData.Cellar), outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(mlContext.Transforms.Conversion
                .ConvertType($"{nameof(ImmoItemData.Garden)}Converted", nameof(ImmoItemData.Garden), outputKind: Microsoft.ML.Data.DataKind.Single))

            .Append(mlContext.Transforms.Concatenate("Features",
                $"{nameof(ImmoItemData.Regio1)}Encoded",
                $"{nameof(ImmoItemData.Regio2)}Encoded",
                $"{nameof(ImmoItemData.Regio3)}Encoded",
                $"{nameof(ImmoItemData.LivingSpace)}Normalized",
                $"{nameof(ImmoItemData.NoRooms)}Normalized",
                $"{nameof(ImmoItemData.NewlyConst)}Converted",
                $"{nameof(ImmoItemData.Balcony)}Converted",
                $"{nameof(ImmoItemData.HasKitchen)}Converted",
                $"{nameof(ImmoItemData.Cellar)}Converted",
                $"{nameof(ImmoItemData.Garden)}Converted"
            ))

            .Append(mlContext.Transforms.CopyColumns("Label", nameof(ImmoItemData.BaseRent)))
            .Append(mlContext.Regression.Trainers.FastTree());

        var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        var model = pipeline.Fit(trainTestData.TrainSet);

        mlContext.Model.Save(model, dataView.Schema, "ImmoaModel.zip");

        var predicts = model.Transform(trainTestData.TestSet);
        var metrics = mlContext.Regression.Evaluate(predicts);

        Console.WriteLine($"Model has been created!");
        Console.WriteLine($"RSquared: {metrics.RSquared:0.##}");
        Console.WriteLine($"Mean Absolute Error:{metrics.MeanAbsoluteError.ToString("n0")}");
        Console.WriteLine($"Mean Squared Error: {metrics.MeanSquaredError.ToString("n0")}");
        Console.WriteLine($"LossFunction: {metrics.LossFunction.ToString("n0")}");
        Console.WriteLine($"Root Mean Squared Error(RMSE): {metrics.RootMeanSquaredError.ToString("n0")}");
    }
}