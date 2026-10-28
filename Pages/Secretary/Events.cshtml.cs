using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class EventsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public EventsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Event> EventsList { get; set; } = new List<Event>();
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

            EventsList = await _context.Events
                .Where(e => e.SocietyId == secretary.SocietyId)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int eventId)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Login");

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId && e.SocietyId == secretary.SocietyId);

            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
                SuccessMessage = "Event deleted successfully.";
            }

            return await OnGetAsync();
        }
    }
}