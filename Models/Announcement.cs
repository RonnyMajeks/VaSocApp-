using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Announcement
{
    public int AnnouncementId { get; set; }

    public int? SocietyId { get; set; }

    public int? CreatedBy { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime DatePosted { get; set; }

    public virtual Person? CreatedByNavigation { get; set; }

    public virtual Society? Society { get; set; }
}
