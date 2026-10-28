using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
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
        public int? SocietyId { get; set; }

       
        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public DateOnly Schedule { get; set; }
        [BindProperty] public string? MeetingTime { get; set; }
        [BindProperty] public string? Description { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var chairperson = await _context.Chairpersons
                .FirstOrDefaultAsync(c => c.PersonId == personId);

            if (chairperson == null || chairperson.SocietyId == null)
            {
                ErrorMessage = "You are not assigned to any society.";
                return Page();
            }

            SocietyId = chairperson.SocietyId;

            var society = await _context.Societies
                .FirstOrDefaultAsync(s => s.SocietyId == chairperson.SocietyId);

            SocietyName = society?.SocietyName ?? "Unknown Society";

            MeetingsList = await _context.Meetings
                .Where(m => m.SocietyId == chairperson.SocietyId)
                .Include(m => m.Secretary)
                    .ThenInclude(s => s != null ? s.Person : null)
                .OrderByDescending(m => m.Schedule)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostScheduleMeetingAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == personId);
            if (chairperson == null || chairperson.SocietyId == null)
            {
                ErrorMessage = "You are not assigned to any society.";
                return await LoadPageAndReturn();
            }

            // Validation
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Meeting title is required.";
                return await LoadPageAndReturn();
            }

            if (Schedule < DateOnly.FromDateTime(DateTime.Today))
            {
                ErrorMessage = "Meeting schedule cannot be in the past.";
                return await LoadPageAndReturn();
            }

            try
            {
                var meeting = new Meeting
                {
                    SocietyId = chairperson.SocietyId.Value,
                    Title = Title,
                    Schedule = Schedule,
                    MeetingTime = MeetingTime,
                    Description = Description
                };

                _context.Meetings.Add(meeting);
                await _context.SaveChangesAsync();

                SuccessMessage = $"Meeting '{Title}' scheduled successfully!";

               //clearing the form 
                Title = "";
                Schedule = default;
                MeetingTime = "";
                Description = "";

                return await LoadPageAndReturn();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error scheduling meeting: " + ex.Message;
                return await LoadPageAndReturn();
            }
        }

        private async Task<IActionResult> LoadPageAndReturn()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == personId);

            if (chairperson != null && chairperson.SocietyId != null)
            {
                SocietyId = chairperson.SocietyId;
                var society = await _context.Societies.FirstOrDefaultAsync(s => s.SocietyId == chairperson.SocietyId);
                SocietyName = society?.SocietyName ?? "Unknown Society";

                MeetingsList = await _context.Meetings
                    .Where(m => m.SocietyId == chairperson.SocietyId)
                    .Include(m => m.Secretary)
                        .ThenInclude(s => s != null ? s.Person : null)
                    .OrderByDescending(m => m.Schedule)
                    .ToListAsync();
            }

            return Page();
        }
    }
}