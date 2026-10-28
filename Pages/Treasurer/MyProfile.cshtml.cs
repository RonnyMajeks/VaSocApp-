using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Treasurer
{
    public class MyProfileModel : PageModel
    {
        private readonly VaSocDbContext _context;
        public MyProfileModel(VaSocDbContext context) { _context = context; }
        public Person? PersonRecord { get; set; }
        public string? SocietyName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");
            PersonRecord = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            var treas = await _context.Treasurers.Include(t => t.Society).FirstOrDefaultAsync(t => t.PersonId == personId);
            if (treas?.Society != null) SocietyName = treas.Society.SocietyName;
            return Page();
        }
    }
}