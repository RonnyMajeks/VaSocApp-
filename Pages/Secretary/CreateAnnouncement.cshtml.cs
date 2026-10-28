using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class CreateAnnouncementModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public CreateAnnouncementModel(VaSocDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public string Message { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Secretary/Announcements");

            var newAnnouncement = new Announcement
            {
                Title = Title,
                Message = Message,
                SocietyId = secretary.SocietyId.Value,
                DatePosted = DateTime.UtcNow
            };

            _context.Announcements.Add(newAnnouncement);
            await _context.SaveChangesAsync();

            SuccessMessage = "Announcement posted successfully!";
            Title = ""; Message = "";

            return Page();
        }
    }
}