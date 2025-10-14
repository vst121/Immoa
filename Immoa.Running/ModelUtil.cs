namespace Immoa.Running;

public class ModelUtil
{
    public static bool TrainModel()
    {
        ModelTrainer modelTrainer = new();
        modelTrainer.Train();

        return true;
    }

    public static int UseModel(ImmoItemModelDto immoItem)
    {
        MLContext mlContext = new();

        ITransformer trainedModel =
            mlContext.Model.Load("D:\\DotNetProjects10\\Immoa\\Immoa.Running\\bin\\Debug\\net10.0\\ImmoaModel.zip",
            out _);

        var predictEngine = mlContext.Model.CreatePredictionEngine<ImmoItemModelDto, BaseRentPredict>(trainedModel);

        var predict1Result = predictEngine.Predict(immoItem);

        return Convert.ToInt32(predict1Result.BaseRent);
    }
}
