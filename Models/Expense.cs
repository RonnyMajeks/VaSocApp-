using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class Expense
{
    public int ExpenseId { get; set; }

    public int? SocietyId { get; set; }

    public decimal? Amount { get; set; }

    public string? Description { get; set; }

    public DateOnly? DateRecorded { get; set; }

    public virtual Society? Society { get; set; }
}
