using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class EventsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public EventsModel(VaSocDbContext context)
        {
            _context = context;
        }

        // List of all events
        public IList<Event> EventsList { get; set; } = new List<Event>();

        // Tracks the current user's RSVP status for each event (e.g., "GOING" or "NOT_GOING")
        public Dictionary<int, string> UserRsvpStatus { get; set; } = new Dictionary<int, string>();

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // 1. Get the logged-in student's Campus ID
            var student = await _context.Students.FirstOrDefaultAsync(s => s.PersonId == personId);
            int? myCampusId = student?.CampusId;

            // 2. Get ONLY events from societies on the student's campus
            EventsList = await _context.Events
                .Include(e => e.Society)
                .Where(e => e.Society.CampusId == myCampusId) // CAMPUS FILTERING
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            // 3. Check which events the user has already RSVP'd to
            if (EventsList.Any())
            {
                var eventIds = EventsList.Select(e => e.EventId).ToList();

                var rsvps = await _context.Rsvps
                    .Where(r => r.PersonId == personId && eventIds.Contains(r.EventId))
                    .ToListAsync();

                // Store the status in our dictionary for the view to use
                foreach (var rsvp in rsvps)
                {
                    UserRsvpStatus[rsvp.EventId] = rsvp.UserStatus;
                }
            }

            return Page();
        }

        // Handle the RSVP button click
        public async Task<IActionResult> OnPostRsvpAsync(int eventId, string status)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // Verify the event belongs to the student's campus (security check)
            var student = await _context.Students.FirstOrDefaultAsync(s => s.PersonId == personId);
            var eventItem = await _context.Events.Include(e => e.Society).FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem?.Society.CampusId != student?.CampusId)
            {
                return BadRequest("You can only RSVP to events from your campus.");
            }

            // Check if user already RSVP'd to this event
            var existingRsvp = await _context.Rsvps
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.PersonId == personId);

            if (existingRsvp != null)
            {
                // Update existing status
                existingRsvp.UserStatus = status;
            }
            else
            {
                // Create new RSVP
                _context.Rsvps.Add(new Rsvp
                {
                    EventId = eventId,
                    PersonId = personId.Value,
                    UserStatus = status
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(); // Refresh the page to show new status
        }
    }
}