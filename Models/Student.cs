using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int PersonId { get; set; }

    public string StudentNumber { get; set; } = null!;

    public int? FacultyId { get; set; }

    public int? CampusId { get; set; }

    public int? YearOfStudy { get; set; }

    public int? Cid { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual Culture? CidNavigation { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual Person Person { get; set; } = null!;
}
