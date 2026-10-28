using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class StudentAffair
{
    public int AffairId { get; set; }

    public int PersonId { get; set; }

    public int? CampusId { get; set; }

    public int? Univid { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual University? Univ { get; set; }
}
