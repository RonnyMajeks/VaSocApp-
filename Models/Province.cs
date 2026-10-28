using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Province
{
    public int Provid { get; set; }

    public string ProvinceName { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
