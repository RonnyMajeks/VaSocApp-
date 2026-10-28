using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class SecretaryDashboardModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public SecretaryDashboardModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? FullName { get; set; }
        public string? SocietyName { get; set; }
        public int? SocietyId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            FullName = HttpContext.Session.GetString("FullName");

            // Find which society this secretary belongs to, including Campus details
            var secretaryRecord = await _context.Secretaries
                .Include(s => s.Society)
                    .ThenInclude(soc => soc != null ? soc.Campus : null)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            // FIX: Added .HasValue check to prevent the nullable warning
            if (secretaryRecord != null && secretaryRecord.Society != null && secretaryRecord.SocietyId.HasValue)
            {
                SocietyName = secretaryRecord.Society.SocietyName;
                SocietyId = secretaryRecord.SocietyId.Value; // Now the compiler knows it's safe

                // Store society info in session for other pages to use
                HttpContext.Session.SetInt32("SecretarySocietyId", SocietyId.Value);
                HttpContext.Session.SetString("SecretarySocietyName", SocietyName);

                // Get University and Campus info for ViewData (for the layout dropdown)
                if (secretaryRecord.Society.Campus != null)
                {
                    var campus = secretaryRecord.Society.Campus;
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