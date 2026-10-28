using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Treasurer
{
    public class AnnouncementsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public AnnouncementsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }
        public IList<Announcement> AnnouncementsList { get; set; } = new List<Announcement>();

        // Form properties for posting a new announcement
        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public string Message { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var treasurer = await _context.Treasurers
                .Include(t => t.Society)
                .FirstOrDefaultAsync(t => t.PersonId == personId);

            if (treasurer == null || treasurer.SocietyId == null)
            {
                SocietyName = "No Society Assigned";
                return Page();
            }

            SocietyName = treasurer.Society?.SocietyName ?? "Unknown Society";
            int? socId = treasurer.SocietyId;

            // Load announcements for this society
            AnnouncementsList = await _context.Announcements
                .Where(a => a.SocietyId == socId)
                .OrderByDescending(a => a.DatePosted)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            var treasurer = await _context.Treasurers.FirstOrDefaultAsync(t => t.PersonId == personId);

            if (treasurer == null || treasurer.SocietyId == null) return RedirectToPage();

            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Message))
            {
                ModelState.AddModelError("", "Title and Message are required.");
                return await OnGetAsync();
            }

            var newAnnouncement = new Announcement
            {
                SocietyId = treasurer.SocietyId.Value,
                Title = Title,
                Message = Message,
                DatePosted = DateTime.Now
            };

            _context.Announcements.Add(newAnnouncement);
            await _context.SaveChangesAsync();

            SuccessMessage = "Announcement posted successfully!";
            Title = "";
            Message = "";

            return await OnGetAsync();
        }
    }
}