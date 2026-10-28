using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class MyProfileModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public MyProfileModel(VaSocDbContext context)
        {
            _context = context;
        }

        public Person? PersonRecord { get; set; }
        public string? SocietyName { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // Get the person's details
            PersonRecord = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);

            if (PersonRecord == null)
            {
                ErrorMessage = "Profile not found.";
                return Page();
            }

            // Get the society they are secretary of
            var secretary = await _context.Secretaries
                .Include(s => s.Society)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (secretary?.Society != null)
            {
                SocietyName = secretary.Society.SocietyName;
            }

            return Page();
        }
    }
}