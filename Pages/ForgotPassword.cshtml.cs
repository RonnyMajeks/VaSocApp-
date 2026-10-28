using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly VaSocDbContext _context;
        public ForgotPasswordModel(VaSocDbContext context) { _context = context; }

        [BindProperty] public string Username { get; set; } = string.Empty;
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Please enter your username or student number.";
                return Page();
            }

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Username == Username || p.Email == Username);
            if (person == null)
            {
                ErrorMessage = "No account found with that username or email.";
                return Page();
            }

            // 1. Generate a random 6-digit OTP
            string otpCode = Random.Shared.Next(100000, 999999).ToString();
            DateTime expiresAt = DateTime.Now.AddMinutes(10);

            // 2. Save to Database
            var otpRecord = new PasswordResetOtp
            {
                PersonId = person.PersonId,
                OtpCode = otpCode,
                ExpiresAt = expiresAt,
                IsUsed = false,
                CreatedAt = DateTime.Now
            };
            _context.PasswordResetOtps.Add(otpRecord);
            await _context.SaveChangesAsync();

            // 3. Store in TempData to pass to the next page
            TempData["ResetPersonId"] = person.PersonId;
            TempData["TestOtpCode"] = otpCode; // Pass OTP to VerifyOtp page

            return RedirectToPage("/VerifyOtp");
        }
    }
}