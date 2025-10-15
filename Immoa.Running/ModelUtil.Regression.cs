namespace Immoa.Running;

public partial class ModelUtil
{
    public static bool TrainRegressionModel()
    {
        ModelTrainingService.TrainRegression();

        return true;
    }

    public static int UseRegressionModel(ApartmentRegressionData apartmentRegressionData)
    {
        MLContext mlContext = new();

        ITransformer trainedModel =
            mlContext.Model.Load("D:\\DotNetProjects10\\Immoa\\Immoa.Running\\bin\\Debug\\net10.0\\ImmoaRegressionModel.zip",
            out _);

        var predictEngine = mlContext.Model.CreatePredictionEngine<ApartmentRegressionData, ApartmentRegressionPrediction>(trainedModel);

        var predictResult = predictEngine.Predict(apartmentRegressionData);

        return Convert.ToInt32(predictResult.PredictedBaseRent);
    }
}
