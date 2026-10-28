using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class AnnouncementsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public AnnouncementsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Announcement> AnnouncementsList { get; set; } = new List<Announcement>();

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.PersonId == personId);
            int? myCampusId = student?.CampusId;

            var campusSocietyIds = await _context.Societies
                .Where(s => s.CampusId == myCampusId)
                .Select(s => s.SocietyId)
                .ToListAsync();

            AnnouncementsList = await _context.Announcements
                .Include(a => a.Society)
                .Where(a => campusSocietyIds.Contains(a.SocietyId ?? 0) && !a.Title.StartsWith("[TREASURER REPORT]"))
                .OrderByDescending(a => a.DatePosted)
                .ToListAsync();

            return Page();
        }
    }
}