namespace Immoa.Training;

public partial class ModelTrainingService
{
    public static void TrainClustering()
    {
        var mlContext = new MLContext();

        var data = DataManager.LoadAllData() 
            .Select(a => new ApartmentClusteringData
            {
                Regio1 = a.Regio1,
                Regio2 = a.Regio2,
                Regio3 = a.Regio3,
                LivingSpace = a.LivingSpace,
                NoRooms = a.NoRooms,
                BaseRent = a.BaseRent,
                Cellar = a.Cellar
            })
            .ToList();

        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(
                [
                    new InputOutputColumnPair("Regio1Encoded", nameof(ApartmentClusteringData.Regio1)),
                    //new InputOutputColumnPair("Regio2Encoded", nameof(ApartmentClusteringData.Regio2)),
                    //new InputOutputColumnPair("Regio3Encoded", nameof(ApartmentClusteringData.Regio3))
                ])
            .Append(mlContext.Transforms.NormalizeMinMax(
                [
                    new InputOutputColumnPair("LivingSpaceNormalized", nameof(ApartmentClusteringData.LivingSpace)),
                    new InputOutputColumnPair("NoRoomsNormalized", nameof(ApartmentClusteringData.NoRooms)),
                    new InputOutputColumnPair("BaseRentNormalized", nameof(ApartmentClusteringData.BaseRent))
                ]))
            .Append(mlContext.Transforms.Conversion.ConvertType(
                $"{nameof(ApartmentClusteringData.Cellar)}Converted", nameof(ApartmentClusteringData.Cellar), DataKind.Single))
            .Append(mlContext.Transforms.Concatenate(
                "Features",
                "Regio1Encoded",
                //"Regio2Encoded",
                //"Regio3Encoded",
                "LivingSpaceNormalized",
                "NoRoomsNormalized",
                "BaseRentNormalized",
                "CellarConverted"))
            .Append(mlContext.Clustering.Trainers.KMeans(
                featureColumnName: "Features",
                numberOfClusters: 5)); // Try k=5 initially

        // Step 3: Train model
        var model = pipeline.Fit(dataView);

        // Step 4: Predict clusters
        var predictions = model.Transform(dataView);
        var clusteredData = mlContext.Data.CreateEnumerable<ApartmentClusteringData>(dataView, reuseRowObject: false)
            .Zip(mlContext.Data.CreateEnumerable<ApartmentClusteringPrediction>(predictions, reuseRowObject: false),
                (data, pred) => new { Data = data, Cluster = pred.PredictedClusterId })
            .ToList();

        //// Step 5: Analyze clusters
        //Console.WriteLine("Cluster Analysis:");
        //var clusterStats = clusteredData
        //    .GroupBy(x => x.Cluster)
        //    .Select(g => new
        //    {
        //        ClusterId = g.Key,
        //        Count = g.Count(),
        //        AvgBaseRent = g.Average(x => x.Data.BaseRent),
        //        AvgLivingSpace = g.Average(x => x.Data.LivingSpace),
        //        AvgNoRooms = g.Average(x => x.Data.NoRooms),
        //        CellarPercentage = g.Average(x => x.Data.Cellar == true ? 1.0 : 0.0) * 100,
        //        TopRegio1 = g.GroupBy(x => x.Data.Regio1).OrderByDescending(x => x.Count()).First().Key
        //    });

        //foreach (var cluster in clusterStats.OrderBy(x => x.ClusterId))
        //{
        //    Console.WriteLine($"Cluster {cluster.ClusterId}:");
        //    Console.WriteLine($"  Count: {cluster.Count}");
        //    Console.WriteLine($"  Avg BaseRent: {cluster.AvgBaseRent:0.##}");
        //    Console.WriteLine($"  Avg LivingSpace: {cluster.AvgLivingSpace:0.##}");
        //    Console.WriteLine($"  Avg NoRooms: {cluster.AvgNoRooms:0.##}");
        //    Console.WriteLine($"  Cellar Presence: {cluster.CellarPercentage:0.##}%");
        //    Console.WriteLine($"  Top Regio1: {cluster.TopRegio1}");
        //}

        // Step 6: Evaluate (Silhouette Score approximation)
        // ML.NET doesn't provide Silhouette Score natively, so approximate with WSS
        var metrics = mlContext.Clustering.Evaluate(predictions, scoreColumnName: "Score", featureColumnName: "Features");
        Console.WriteLine($"Average Distance (Within Cluster Sum of Squares): {metrics.AverageDistance:0.##}");
        Console.WriteLine($"Davies Bouldin Index: {metrics.DaviesBouldinIndex:0.##}");

        // Step 7: Save model
        mlContext.Model.Save(model, dataView.Schema, "ImmoaClusteringModel.zip");

        Console.WriteLine($"Model for Clustering has been created!");

        //// Optional: Compare with classification labels
        //var dataWithLabels = DataManager.LoadAllData()
        //    .Where(a => a.LivingSpace > 0 && a.BaseRent != null && a.LivingSpace != null)
        //    .Select(a => new
        //    {
        //        Category = (a.BaseRent / a.LivingSpace) switch
        //        {
        //            < 10 => "Budget",
        //            < 20 => "Standard",
        //            _ => "Luxury"
        //        },
        //        Data = a
        //    })
        //    .Zip(clusteredData, (label, cluster) => new { label.Category, Cluster = cluster.Cluster })
        //    .GroupBy(x => new { x.Cluster, x.Category })
        //    .Select(g => new
        //    {
        //        ClusterId = g.Key.Cluster,
        //        Category = g.Key.Category,
        //        Count = g.Count()
        //    });

        //Console.WriteLine("\nCluster vs. Classification Labels:");
        //foreach (var item in dataWithLabels.OrderBy(x => x.ClusterId).ThenBy(x => x.Category))
        //{
        //    Console.WriteLine($"Cluster {item.ClusterId}, {item.Category}: {item.Count}");
        //}
    }
}