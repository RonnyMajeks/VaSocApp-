using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class EditEventModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public EditEventModel(VaSocDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public string? Description { get; set; }
        [BindProperty] public string? Venue { get; set; }
        [BindProperty] public DateOnly EventDate { get; set; }

        // FIX: Changed TimeSpan? to TimeOnly? to match database
        [BindProperty] public TimeOnly? EventTime { get; set; }

        [BindProperty] public bool IsMembersOnly { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Secretary/Events");

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id && e.SocietyId == secretary.SocietyId);
            if (ev == null) return NotFound();

            // Populate form
            Title = ev.Title;
            Description = ev.Description;
            Venue = ev.Venue;
            EventDate = ev.EventDate;
            EventTime = ev.EventTime; // Now works because both are TimeOnly?
            IsMembersOnly = ev.IsMembersOnly;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Secretary/Events");

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id && e.SocietyId == secretary.SocietyId);
            if (ev == null) return NotFound();

            // Update properties
            ev.Title = Title;
            ev.Description = Description;
            ev.Venue = Venue;
            ev.EventDate = EventDate;
            ev.EventTime = EventTime; // Now works because both are TimeOnly?
            ev.IsMembersOnly = IsMembersOnly;

            await _context.SaveChangesAsync();
            SuccessMessage = "Event updated successfully!";

            return Page();
        }
    }
}