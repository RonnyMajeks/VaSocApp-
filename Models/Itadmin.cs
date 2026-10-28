using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Itadmin
{
    public int ItadminId { get; set; }

    public int PersonId { get; set; }

    public virtual Person Person { get; set; } = null!;
}
