using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
{
    public class ReportsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public ReportsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }
        public int ActiveMemberCount { get; set; }
        public int AnnouncementCount { get; set; }
        public int MeetingCount { get; set; }
        public IList<Announcement> RecentUpdates { get; set; } = new List<Announcement>();

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // 1. Get the Chairperson record
            var chairperson = await _context.Chairpersons
                .FirstOrDefaultAsync(c => c.PersonId == personId);

            if (chairperson == null || chairperson.SocietyId == null)
            {
                SocietyName = "No Society Assigned";
                return Page();
            }

            // 2. FIX: Get the Society Name directly from the Societies table
            var society = await _context.Societies
                .FirstOrDefaultAsync(s => s.SocietyId == chairperson.SocietyId);

            SocietyName = society?.SocietyName ?? "Unknown Society";
            int socId = chairperson.SocietyId.Value;

            // 3. Calculate live stats
            ActiveMemberCount = await _context.Memberships.CountAsync(m => m.SocietyId == socId && m.Status == "ACCEPTED");
            AnnouncementCount = await _context.Announcements.CountAsync(a => a.SocietyId == socId);
            MeetingCount = await _context.Meetings.CountAsync(m => m.SocietyId == socId);

            // 4. Get recent updates
            RecentUpdates = await _context.Announcements
                .Where(a => a.SocietyId == socId)
                .OrderByDescending(a => a.DatePosted)
                .Take(5)
                .ToListAsync();

            return Page();
        }
    }
}