using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
{
    public class AnnouncementsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public AnnouncementsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Announcement> AnnouncementsList { get; set; } = new List<Announcement>();
        public string? SocietyName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == personId);
            if (chairperson == null || chairperson.SocietyId == null) return Page();

            var society = await _context.Societies.FirstOrDefaultAsync(s => s.SocietyId == chairperson.SocietyId);
            SocietyName = society?.SocietyName;

            AnnouncementsList = await _context.Announcements
                .Where(a => a.SocietyId == chairperson.SocietyId)
                .OrderByDescending(a => a.DatePosted)
                .ToListAsync();

            return Page();
        }
    }
}