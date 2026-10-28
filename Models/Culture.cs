using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Culture
{
    public int Cid { get; set; }

    public string CultureName { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
