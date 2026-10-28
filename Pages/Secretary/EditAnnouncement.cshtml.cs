using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class EditAnnouncementModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public EditAnnouncementModel(VaSocDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public string Message { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Secretary/Announcements");

            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(a => a.AnnouncementId == id && a.SocietyId == secretary.SocietyId);

            if (announcement == null) return NotFound();

            Title = announcement.Title;
            Message = announcement.Message;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Secretary/Announcements");

            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(a => a.AnnouncementId == id && a.SocietyId == secretary.SocietyId);

            if (announcement == null) return NotFound();

            announcement.Title = Title;
            announcement.Message = Message;
            // Note: We do not update DatePosted here, it keeps the original post time. 
            // If you want to update it to "now", uncomment the line below:
            // announcement.DatePosted = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            SuccessMessage = "Announcement updated successfully!";

            return Page();
        }
    }
}