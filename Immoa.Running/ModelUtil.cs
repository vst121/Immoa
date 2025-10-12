namespace Immoa.Running;

public class ModelUtil
{
    public static bool TrainModel()
    {
        ModelTrainer modelTrainer = new();
        modelTrainer.Train();

        return true;
    }

    public static int UseModel()
    {
        MLContext mlContext = new();

        ITransformer trainedModel =
            mlContext.Model.Load("D:\\DotNetProjects10\\Immoa\\Immoa.Running\\bin\\Debug\\net10.0\\ImmoModel.zip",
            out _);

        var predictEngine = mlContext.Model.CreatePredictionEngine<ImmoItemData, BaseRentPredict>(trainedModel);

        var predictResult = predictEngine.Predict(new ImmoItemData
        {
            GeoPlz = 68169,
            Lift = false, 
            Balcony = true,
        });

        Console.WriteLine($"Predicted Base Rent: {Convert.ToInt32(predictResult.BaseRent).ToString("n0")}");


        return Convert.ToInt32(predictResult.BaseRent);
    }
}
