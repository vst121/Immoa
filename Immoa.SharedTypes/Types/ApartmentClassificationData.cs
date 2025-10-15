namespace Immoa.SharedTypes.Types;

public class ApartmentClassificationData
{
    public string Regio1 { get; set; }
    public string Regio2 { get; set; }
    public string Regio3 { get; set; }
    public Single LivingSpace { get; set; }
    public Single NoRooms { get; set; }
    public Single BaseRent { get; set; }
    public string Category { get; set; } // Budget / Standard / Luxury
}

public class ApartmentClassificationPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedCategory { get; set; }
}