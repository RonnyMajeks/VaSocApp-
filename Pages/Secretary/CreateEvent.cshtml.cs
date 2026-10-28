using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class CreateEventModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public CreateEventModel(VaSocDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public string? Description { get; set; }
        [BindProperty] public string? Venue { get; set; }
        [BindProperty] public DateOnly EventDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        // FIX: Changed TimeSpan? to TimeOnly? to match database
        [BindProperty] public TimeOnly? EventTime { get; set; }

        [BindProperty] public bool IsMembersOnly { get; set; }

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
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Secretary/Events");

            var newEvent = new Event
            {
                Title = Title,
                Description = Description,
                Venue = Venue,
                EventDate = EventDate,
                EventTime = EventTime,
                IsMembersOnly = IsMembersOnly,
                //  Added.Value because SocietyId in Events table is not nullable
                SocietyId = secretary.SocietyId.Value
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            SuccessMessage = "Event created successfully!";

            // Clear form
            Title = ""; Description = ""; Venue = ""; IsMembersOnly = false;

            return Page();
        }
    }
}