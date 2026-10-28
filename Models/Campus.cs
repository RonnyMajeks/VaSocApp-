using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Campus
{
    public int CampusId { get; set; }

    public string CampusName { get; set; } = null!;

    public int? LocationId { get; set; }

    public int? Univid { get; set; }

    public virtual Location? Location { get; set; }

    public virtual ICollection<Society> Societies { get; set; } = new List<Society>();

    public virtual ICollection<StudentAffair> StudentAffairs { get; set; } = new List<StudentAffair>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual University? Univ { get; set; }
}
