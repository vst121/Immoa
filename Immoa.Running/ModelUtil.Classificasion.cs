namespace Immoa.Running;

public partial class ModelUtil
{
    public static bool TrainClassificationModel()
    {
        ModelTrainingService.TrainClassification();

        return true;
    }

    public static string UseClassificationModel(ApartmentClassificationData apartmentClassificationData)
    {
        MLContext mlContext = new();

        ITransformer trainedModel =
            mlContext.Model.Load("D:\\DotNetProjects10\\Immoa\\Immoa.Running\\bin\\Debug\\net10.0\\ImmoaClassificationModel.zip",
            out _);

        var predictEngine = mlContext.Model.CreatePredictionEngine<ApartmentClassificationData, ApartmentClassificationPrediction>(trainedModel);

        var predictResult = predictEngine.Predict(apartmentClassificationData);

        return predictResult.PredictedCategory;
    }
}
