using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Event
{
    public int EventId { get; set; }

    public int SocietyId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Venue { get; set; }

    public int? LocationId { get; set; }

    public DateOnly EventDate { get; set; }

    public TimeOnly? EventTime { get; set; }

    public bool IsMembersOnly { get; set; }

    public virtual Location? Location { get; set; }

    public virtual ICollection<Rsvp> Rsvps { get; set; } = new List<Rsvp>();

    public virtual Society Society { get; set; } = null!;
}
