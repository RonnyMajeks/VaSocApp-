using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string? MapUrl { get; set; }

    public int? CityId { get; set; }

    public virtual ICollection<Campus> Campuses { get; set; } = new List<Campus>();

    public virtual City? City { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
