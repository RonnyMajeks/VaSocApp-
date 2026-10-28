using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Membership
{
    public int MembershipId { get; set; }

    public int? StudentId { get; set; }

    public int? SocietyId { get; set; }

    public string Status { get; set; } = null!;

    public string? ReasonForJoining { get; set; }

    public DateTime RequestDate { get; set; }

    public virtual Society? Society { get; set; }

    public virtual Student? Student { get; set; }
}
