using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Chairperson
{
    public int ChairpersonId { get; set; }

    public int? PersonId { get; set; }

    public int? SocietyId { get; set; }

    public virtual Person? Person { get; set; }

    public virtual Society? Society { get; set; }
}
