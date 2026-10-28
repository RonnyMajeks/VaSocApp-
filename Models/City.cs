using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public int? Provid { get; set; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual Province? Prov { get; set; }
}
