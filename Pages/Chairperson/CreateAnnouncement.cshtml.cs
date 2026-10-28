using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
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
            if (HttpContext.Session.GetInt32("PersonId") == null) return RedirectToPage("/Login");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == personId);
            if (chairperson == null || chairperson.SocietyId == null) return RedirectToPage("/Chairperson/Announcements");

            var newAnnouncement = new Announcement
            {
                Title = Title,
                Message = Message,
                SocietyId = chairperson.SocietyId.Value,
                DatePosted = DateTime.UtcNow
            };

            _context.Announcements.Add(newAnnouncement);
            await _context.SaveChangesAsync();

            SuccessMessage = "Announcement posted successfully!";
            Title = ""; Message = ""; // Clear form

            return Page();
        }
    }
}