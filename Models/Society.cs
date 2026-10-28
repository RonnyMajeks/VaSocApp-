using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Society
{
    public int SocietyId { get; set; }

    public string SocietyName { get; set; } = null!;

    public string? Description { get; set; }

    public int? CampusId { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<Chairperson> Chairpeople { get; set; } = new List<Chairperson>();

    public virtual ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<Secretary> Secretaries { get; set; } = new List<Secretary>();

    public virtual ICollection<Treasurer> Treasurers { get; set; } = new List<Treasurer>();
}
