using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Rsvp
{
    public int RsvpId { get; set; }

    public int EventId { get; set; }

    public int PersonId { get; set; }

    public string UserStatus { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
