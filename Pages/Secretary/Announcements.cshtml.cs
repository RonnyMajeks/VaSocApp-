using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
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
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries
                .Include(s => s.Society)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (secretary == null || secretary.SocietyId == null) return Page();

            SocietyName = secretary.Society.SocietyName;

            AnnouncementsList = await _context.Announcements
                .Where(a => a.SocietyId == secretary.SocietyId && !a.Title.StartsWith("[TREASURER REPORT]"))
                .OrderByDescending(a => a.DatePosted)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int announcementId)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Login");

            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId && a.SocietyId == secretary.SocietyId);

            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
                SuccessMessage = "Announcement deleted successfully.";
            }

            return await OnGetAsync();
        }
    }
}