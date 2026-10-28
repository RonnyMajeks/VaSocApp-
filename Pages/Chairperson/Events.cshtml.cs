using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
{
    public class EventsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public EventsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<VaSocApp_System.Models.Event> EventsList { get; set; } = new List<VaSocApp_System.Models.Event>();
        public string? SocietyName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == personId);
            if (chairperson == null || chairperson.SocietyId == null) return Page();

            var society = await _context.Societies.FirstOrDefaultAsync(s => s.SocietyId == chairperson.SocietyId);
            SocietyName = society?.SocietyName;

            EventsList = await _context.Events
                .Where(e => e.SocietyId == chairperson.SocietyId)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();

            return Page();
        }
    }
}