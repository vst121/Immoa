namespace Immoa.Running;

public partial class ModelUtil
{
    public static bool TrainClusteringModel()
    {
        ModelTrainingService.TrainClustering();

        return true;
    }

    public static int UseClusteringModel(ApartmentClusteringData apartmentClusteringData)
    {
        MLContext mlContext = new();

        ITransformer trainedModel =
            mlContext.Model.Load("D:\\DotNetProjects10\\Immoa\\Immoa.Running\\bin\\Debug\\net10.0\\ImmoaClusteringModel.zip",
            out _);

        var predictEngine = mlContext.Model.CreatePredictionEngine<ApartmentClusteringData, ApartmentClusteringPrediction>(trainedModel);

        var predictResult = predictEngine.Predict(apartmentClusteringData);

        return predictResult.PredictedClusterId;
    }
}
