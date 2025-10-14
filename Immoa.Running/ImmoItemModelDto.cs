using System;
using System.Collections.Generic;
using System.Text;

namespace Immoa.Running;

public class ImmoItemModelDto
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
}
