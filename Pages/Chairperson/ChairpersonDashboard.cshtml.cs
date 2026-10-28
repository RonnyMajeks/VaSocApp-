using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
{
    public class ChairpersonDashboardModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public ChairpersonDashboardModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? FullName { get; set; }
        public string? SocietyName { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null)
            {
                return RedirectToPage("/Login");
            }

            FullName = HttpContext.Session.GetString("FullName");

            // Fetch chairperson details with society and campus info
            var chairperson = await _context.Chairpersons
                .Include(c => c.Society)
                    .ThenInclude(s => s != null ? s.Campus : null)
                .FirstOrDefaultAsync(c => c.PersonId == personId);

            if (chairperson?.Society != null)
            {
                SocietyName = chairperson.Society.SocietyName;

                // Get university and campus info for ViewData
                if (chairperson.Society.Campus != null)
                {
                    var campus = chairperson.Society.Campus;
                    ViewData["CampusName"] = campus.CampusName;

                    if (campus.Univid.HasValue)
                    {
                        var university = await _context.Universities
                            .FirstOrDefaultAsync(u => u.Univid == campus.Univid);
                        ViewData["UniversityName"] = university?.Uniname;
                    }
                }
            }

            // Get person details for email and username
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