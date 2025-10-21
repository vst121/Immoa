namespace Immoa.Common.Types;

public class ApartmentClassificationData 
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
    public string Category { get; set; } // Budget / Standard / Luxury
}

public class ApartmentClassificationPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedCategory { get; set; }
}