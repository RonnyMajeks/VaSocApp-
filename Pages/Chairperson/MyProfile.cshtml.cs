using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
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

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            PersonRecord = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);

            var chairperson = await _context.Chairpersons
                .Include(c => c.Society)
                .FirstOrDefaultAsync(c => c.PersonId == personId);

            if (chairperson?.Society != null)
            {
                SocietyName = chairperson.Society.SocietyName;
            }

            return Page();
        }
    }
}