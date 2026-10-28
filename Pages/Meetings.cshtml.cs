using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class MeetingsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public MeetingsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Meeting> MeetingsList { get; set; } = new List<Meeting>();

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // Fetch meetings, upcoming first, and include the Society name
            MeetingsList = await _context.Meetings
                .Include(m => m.Society)
                .OrderByDescending(m => m.Schedule)
                .ToListAsync();

            return Page();
        }
    }
}