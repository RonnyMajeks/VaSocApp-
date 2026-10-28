using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class EditProfileModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public EditProfileModel(VaSocDbContext context)
        {
            _context = context;
        }

        // Bind individual properties to match the HTML form exactly
        [BindProperty]
        public string Username { get; set; } = string.Empty;
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string? Phone { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            if (person == null) return NotFound();

            // Load current data into the form fields
            Username = person.Username;
            Email = person.Email;
            Phone = person.Phone;
           
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var personId = HttpContext.Session.GetInt32("PersonId");
            var existingPerson = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);

            if (existingPerson == null) return NotFound();

            // Update only the allowed fields
            existingPerson.Username = Username;
            existingPerson.Email = Email;
            existingPerson.Phone = Phone;
           
            try
            {
                await _context.SaveChangesAsync();

                // Update session info if username changed
                HttpContext.Session.SetString("Username", existingPerson.Username);
                HttpContext.Session.SetString("FullName", $"{existingPerson.FirstName} {existingPerson.LastName}");

                SuccessMessage = "Profile updated successfully.";
            }
            catch (DbUpdateException)
            {
                ErrorMessage = "Error saving changes. Username or Email might already be in use.";
            }

            return Page();
        }
    }
}