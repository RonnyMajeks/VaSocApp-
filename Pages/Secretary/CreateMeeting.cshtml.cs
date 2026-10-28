using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class CreateMeetingModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public CreateMeetingModel(VaSocDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string? Description { get; set; }

        [BindProperty]
        public DateTime Schedule { get; set; } = DateTime.Now;

        [BindProperty]
        public string? MeetingTime { get; set; } // e.g., "14:00"

        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
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
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Meetings");

            var newMeeting = new Meeting
            {
                Title = Title,
                Description = Description,
                Schedule = DateOnly.FromDateTime(Schedule),
                MeetingTime = MeetingTime,
                SocietyId = secretary.SocietyId,
                SecretaryId = secretary.SecretaryId
            };

            _context.Meetings.Add(newMeeting);
            await _context.SaveChangesAsync();

            SuccessMessage = "Meeting scheduled successfully!";

            // Clear form
            Title = "";
            Description = "";
            MeetingTime = "";

            return Page();
        }
    }
}
