using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual ICollection<Chairperson> Chairpeople { get; set; } = new List<Chairperson>();

    public virtual ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();

    public virtual Itadmin? Itadmin { get; set; }

    public virtual ICollection<PasswordResetOtp> PasswordResetOtps { get; set; } = new List<PasswordResetOtp>();

    public virtual ICollection<Rsvp> Rsvps { get; set; } = new List<Rsvp>();

    public virtual ICollection<Secretary> Secretaries { get; set; } = new List<Secretary>();

    public virtual Student? Student { get; set; }

    public virtual StudentAffair? StudentAffair { get; set; }

    public virtual ICollection<Treasurer> Treasurers { get; set; } = new List<Treasurer>();
}
