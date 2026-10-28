using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class MeetingsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public MeetingsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Meeting> MeetingsList { get; set; } = new List<Meeting>();
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
            int societyId = secretary.SocietyId.Value;

            // Get all meetings for this society
            MeetingsList = await _context.Meetings
                .Where(m => m.SocietyId == societyId)
                .OrderByDescending(m => m.Schedule)
                .ToListAsync();

            return Page();
        }

        // Handle Delete button
        public async Task<IActionResult> OnPostDeleteAsync(int meetId)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Login");

            var meeting = await _context.Meetings
                .FirstOrDefaultAsync(m => m.Meetid == meetId && m.SocietyId == secretary.SocietyId);

            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
                await _context.SaveChangesAsync();
                SuccessMessage = "Meeting deleted successfully.";
            }

            return await OnGetAsync();
        }
    }
}