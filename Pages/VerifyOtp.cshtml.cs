using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class VerifyOtpModel : PageModel
    {
        private readonly VaSocDbContext _context;
        public VerifyOtpModel(VaSocDbContext context) { _context = context; }

        [BindProperty] public string OtpCode { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? TestOtpCode { get; set; }

        public void OnGet()
        {
            // Check if we have a person ID. If not, redirect to start.
            if (TempData["ResetPersonId"] == null)
            {
                RedirectToPage("/ForgotPassword");
                return;
            }

            // ️ CRITICAL FIX: Keep TempData alive for the POST request!
            // Without this, TempData is deleted after OnGet finishes.
            TempData.Keep("ResetPersonId");
            TempData.Keep("TestOtpCode");

            // Read the OTP to display it on the screen
            TestOtpCode = TempData["TestOtpCode"]?.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Now this will work because we kept the data alive in OnGet
            if (TempData["ResetPersonId"] == null)
            {
                return RedirectToPage("/ForgotPassword");
            }

            int personId = (int)TempData["ResetPersonId"]!;

            // Query matches your PASSWORD_RESET_OTPS table schema
            var otpRecord = await _context.PasswordResetOtps
                .Where(o => o.PersonId == personId && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
            {
                ErrorMessage = "No valid reset code found. Please request a new one.";
                return Page();
            }

            if (otpRecord.ExpiresAt < DateTime.Now)
            {
                ErrorMessage = "This code has expired. Please request a new one.";
                return Page();
            }

            if (otpRecord.OtpCode != OtpCode)
            {
                ErrorMessage = "Invalid code. Please try again.";
                return Page();
            }

            // Code is valid! Keep PersonId for the ResetPassword page
            TempData["ResetPersonId"] = personId;
            return RedirectToPage("/ResetPassword");
        }
    }
}