using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class University
{
    public int Univid { get; set; }

    public string Uniname { get; set; } = null!;

    public string? StudentDomain { get; set; }

    public string? StaffDomain { get; set; }

    public virtual ICollection<Campus> Campuses { get; set; } = new List<Campus>();

    public virtual ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();

    public virtual ICollection<StudentAffair> StudentAffairs { get; set; } = new List<StudentAffair>();
}
