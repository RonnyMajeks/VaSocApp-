using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class LoginModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public LoginModel(VaSocDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password.";
                return Page();
            }

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Username == Username);

            if (person == null || !person.IsActive)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            if (!BCrypt.Net.BCrypt.Verify(Password, person.PasswordHash))
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            HttpContext.Session.SetInt32("PersonId", person.PersonId);
            HttpContext.Session.SetString("Username", person.Username);
            HttpContext.Session.SetString("FullName", $"{person.FirstName} {person.LastName}");
            HttpContext.Session.SetString("UserType", person.UserType);

            if (person.UserType == "STUDENT")
            {
                return RedirectToPage("/Dashboard");
            }
            else if (person.UserType == "SOCIETY_LEADER")
            {
                var treasurer = await _context.Treasurers.FirstOrDefaultAsync(t => t.PersonId == person.PersonId);
                var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == person.PersonId);
                var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == person.PersonId);

                // FIXED: Route matches the file name TreasuerDashboard.cshtml
                if (treasurer != null) return RedirectToPage("/Treasurer/TreasurerDashboard");
                if (secretary != null) return RedirectToPage("/Secretary/SecretaryDashboard");
                if (chairperson != null) return RedirectToPage("/Chairperson/ChairpersonDashboard");

                return RedirectToPage("/Dashboard");
            }
            else if (person.UserType == "IT_ADMIN")
            {
                return RedirectToPage("/Admin/AdminDashboard");
            }
            else if (person.UserType == "STUDENT_AFFAIRS")
            {
                return RedirectToPage("/StudentAffairs/StudentAffairsDashboard");
            }

            return RedirectToPage("/Dashboard");
        }
    }
}