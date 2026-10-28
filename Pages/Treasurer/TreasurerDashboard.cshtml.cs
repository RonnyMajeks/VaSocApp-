using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Treasurer
{
    public class TreasurerDashboardModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public TreasurerDashboardModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? FullName { get; set; }
        public string? SocietyName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            FullName = HttpContext.Session.GetString("FullName");

            var treasurer = await _context.Treasurers
                .Include(t => t.Society)
                    .ThenInclude(s => s != null ? s.Campus : null)
                .FirstOrDefaultAsync(t => t.PersonId == personId);

            if (treasurer?.Society != null)
            {
                SocietyName = treasurer.Society.SocietyName;

                if (treasurer.Society.Campus != null)
                {
                    var campus = treasurer.Society.Campus;
                    ViewData["CampusName"] = campus.CampusName;

                    if (campus.Univid.HasValue)
                    {
                        var university = await _context.Universities
                            .FirstOrDefaultAsync(u => u.Univid == campus.Univid);
                        ViewData["UniversityName"] = university?.Uniname;
                    }
                }
            }

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            if (person != null)
            {
                ViewData["Username"] = person.Username;
                ViewData["Email"] = person.Email;
            }

            return Page();
        }
    }
}