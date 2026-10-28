using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class SocietiesModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public SocietiesModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Society> SocietiesList { get; set; } = new List<Society>();
        public string? CampusName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (student == null || student.CampusId == null) return Page();

            var campus = await _context.Campuses.FirstOrDefaultAsync(c => c.CampusId == student.CampusId);
            CampusName = campus?.CampusName ?? "Unknown Campus";

            SocietiesList = await _context.Societies
                .Where(s => s.CampusId == student.CampusId)
                .ToListAsync();

            return Page();
        }
    }
}