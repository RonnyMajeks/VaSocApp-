using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly VaSocDbContext _context;
        public ResetPasswordModel(VaSocDbContext context) { _context = context; }

        [BindProperty] public string NewPassword { get; set; } = string.Empty;
        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Check if we have a person ID
            if (TempData["ResetPersonId"] == null)
            {
                RedirectToPage("/ForgotPassword");
                return; // Important: stop execution
            }

            // CRITICAL FIX: Keep TempData alive for the POST request!
            TempData.Keep("ResetPersonId");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                return Page();
            }

            // Check if TempData exists before casting
            if (TempData["ResetPersonId"] == null)
            {
                ErrorMessage = "Session expired. Please request a new password reset.";
                return RedirectToPage("/ForgotPassword");
            }

            int personId = (int)TempData["ResetPersonId"]!;

            var person = await _context.Persons.FindAsync(personId);
            if (person == null)
            {
                ErrorMessage = "User not found.";
                return RedirectToPage("/Login");
            }

            // 1. Update Password
            person.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            // 2. Mark the OTP as used so it can't be reused
            var otpRecord = await _context.PasswordResetOtps
                .Where(o => o.PersonId == personId && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord != null)
            {
                otpRecord.IsUsed = true;
            }

            await _context.SaveChangesAsync();

            // Clear TempData
            TempData.Remove("ResetPersonId");

            SuccessMessage = "Password reset successfully! You can now log in.";
            return RedirectToPage("/Login");
        }
    }
}