using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Budget
{
    public int BudgetId { get; set; }

    public int? SocietyId { get; set; }

    public decimal? AmountAllocated { get; set; }

    public string? AcademicYear { get; set; }

    public DateOnly? DateAllocated { get; set; }

    public virtual Society? Society { get; set; }
}
