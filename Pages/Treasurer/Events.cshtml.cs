using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Treasurer
{
    public class EventsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public EventsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }

        // FIXED: Changed from 'Events' to 'Event' to match your model
        public IList<VaSocApp_System.Models.Event> EventsList { get; set; } = new List<VaSocApp_System.Models.Event>();

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
            int socId = treasurer.SocietyId.Value;

            // Load events for this society
            EventsList = await _context.Events
                .Where(e => e.SocietyId == socId)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();

            return Page();
        }
    }
}