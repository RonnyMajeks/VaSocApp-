using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Meeting
{
    public int Meetid { get; set; }

    public int? SecretaryId { get; set; }

    public int? SocietyId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateOnly? Schedule { get; set; }

    public string? MeetingTime { get; set; }

    public virtual Secretary? Secretary { get; set; }

    public virtual Society? Society { get; set; }
}
