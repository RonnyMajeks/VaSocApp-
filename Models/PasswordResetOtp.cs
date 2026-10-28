using System;
using System.Collections.Generic;

namespace VaSocApp_System.Models;

public partial class PasswordResetOtp
{
    public int OtpId { get; set; }

    public int PersonId { get; set; }

    public string OtpCode { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Person Person { get; set; } = null!;
}
