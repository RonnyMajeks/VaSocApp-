using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyName { get; set; } = null!;

    public int? Univid { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual University? Univ { get; set; }
}
