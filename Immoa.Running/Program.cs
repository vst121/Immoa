
ModelUtil.TrainModel();

var dto1 = new ImmoItemModelDto
{
    Regio1 = "Hessen",
    GeoPlz = 60438,
    Lift = true,
    Balcony = true,
    NoRooms = 4
};

var baseRent1 = ModelUtil.UseModel(dto1);
Console.WriteLine($"Predicted Base Rent: {Convert.ToInt32(baseRent1).ToString("n0")}");

var dto2 = new ImmoItemModelDto
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

var baseRent2 = ModelUtil.UseModel(dto2);
Console.WriteLine($"Predicted Base Rent: {Convert.ToInt32(baseRent2).ToString("n0")}");


Console.WriteLine("Model run!");
Console.ReadLine();
