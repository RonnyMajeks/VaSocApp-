using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.StudentAffairs
{
    public class MyProfileModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public MyProfileModel(VaSocDbContext context)
        {
            _context = context;
        }

        public Person? PersonRecord { get; set; }

        // Form properties for editing
        [BindProperty] public string FirstName { get; set; } = string.Empty;
        [BindProperty] public string LastName { get; set; } = string.Empty;
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string? Phone { get; set; }
        [BindProperty] public string Username { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(bool edit = false)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            PersonRecord = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            if (PersonRecord == null) return NotFound();

            IsEditMode = edit;
            if (edit)
            {
                FirstName = PersonRecord.FirstName;
                LastName = PersonRecord.LastName;
                Email = PersonRecord.Email;
                Phone = PersonRecord.Phone;
                Username = PersonRecord.Username;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);

            if (person == null) return NotFound();

            person.FirstName = FirstName;
            person.LastName = LastName;
            person.Email = Email;
            person.Phone = Phone;
            person.Username = Username;

            try
            {
                await _context.SaveChangesAsync();

                // Update session so the navbar reflects changes immediately
                HttpContext.Session.SetString("Username", person.Username);
                HttpContext.Session.SetString("FullName", $"{person.FirstName} {person.LastName}");

                SuccessMessage = "Profile updated successfully.";
                IsEditMode = false;
                PersonRecord = person; // Refresh the view model
            }
            catch (DbUpdateException)
            {
                ErrorMessage = "Error saving changes. Username or Email may already be in use.";
                IsEditMode = true; // Keep them in edit mode to fix the error
            }

            return Page();
        }
    }
}