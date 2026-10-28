using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Secretary
{
    public int SecretaryId { get; set; }

    public int? PersonId { get; set; }

    public int? SocietyId { get; set; }

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public virtual Person? Person { get; set; }

    public virtual Society? Society { get; set; }
}
