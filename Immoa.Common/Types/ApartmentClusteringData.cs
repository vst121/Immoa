namespace Immoa.Common.Types;

public class ApartmentClusteringData 
{
    public string Regio1 { get; set; }
    public string Regio2 { get; set; }
    public string Regio3 { get; set; }
    public Single LivingSpace { get; set; }
    public Single NoRooms { get; set; }
    public bool NewlyConst { get; set; }
    public bool Balcony { get; set; }
    public bool HasKitchen { get; set; }
    public bool Cellar { get; set; }
    public bool Lift { get; set; }
    public bool Garden { get; set; }
    public Single BaseRent { get; set; }
    public int PredictedClusterId { get; set; }
}

public class ApartmentClusteringPrediction
{
    [ColumnName("PredictedClusterId")]
    public int PredictedClusterId { get; set; }
    [ColumnName("Score")]
    public float[] Distances { get; set; }
}