
// Regression
ModelUtil.TrainRegressionModel();

var regData = new ApartmentRegressionData
{
    Regio1 = "Bayern",
    //GeoPlz = 60438,
    NewlyConst = true,
    LivingSpace = 100,
    Balcony = false,
    HasKitchen = true,  
    Cellar = true,
    Lift = false,
    NoRooms = 1,
    Garden = true,
};

var regPredictedbaseRent = ModelUtil.UseRegressionModel(regData);
Console.WriteLine($"Predicted Base Rent: {Convert.ToInt32(regPredictedbaseRent):n0}");
Console.WriteLine("");

// Classification
ModelUtil.TrainClassificationModel();

var clsData = new ApartmentClassificationData
{
    Regio1 = "Hessen",
    NoRooms = 4
};

var clsPredictedCategory = ModelUtil.UseClassificationModel(clsData);
Console.WriteLine($"Predicted Category: {clsPredictedCategory}");

Console.ReadLine();
