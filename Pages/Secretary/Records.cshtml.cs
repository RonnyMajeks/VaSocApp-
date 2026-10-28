using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;
using System.Text;

namespace VaSocApp_System.Pages.Secretary
{
    public class RecordsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public RecordsModel(VaSocDbContext context)
        {
            _context = context;
        }

        // Data for Charts
        public int TotalAccepted { get; set; }
        public int TotalSuspended { get; set; }
        public int TotalPending { get; set; }

        public List<EventRsvpData> EventRsvps { get; set; } = new List<EventRsvpData>();

        // Data for Interactive Table
        public IList<Membership> AllMembers { get; set; } = new List<Membership>();
        public string? SocietyName { get; set; }

        public class EventRsvpData
        {
            public string EventTitle { get; set; } = "";
            public int GoingCount { get; set; }
            public int NotGoingCount { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries
                .Include(s => s.Society)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (secretary == null || secretary.SocietyId == null) return Page();

            SocietyName = secretary.Society.SocietyName;
            int socId = secretary.SocietyId.Value;

            // 1. Get Member Status Counts for Pie Chart
            TotalAccepted = await _context.Memberships.CountAsync(m => m.SocietyId == socId && m.Status == "ACCEPTED");
            TotalSuspended = await _context.Memberships.CountAsync(m => m.SocietyId == socId && m.Status == "SUSPENDED");
            TotalPending = await _context.Memberships.CountAsync(m => m.SocietyId == socId && m.Status == "PENDING");

            // 2. Get Event RSVP Counts for Bar Chart
            var events = await _context.Events.Where(e => e.SocietyId == socId).ToListAsync();
            foreach (var ev in events)
            {
                var going = await _context.Rsvps.CountAsync(r => r.EventId == ev.EventId && r.UserStatus == "GOING");
                var notGoing = await _context.Rsvps.CountAsync(r => r.EventId == ev.EventId && r.UserStatus == "NOT_GOING");

                EventRsvps.Add(new EventRsvpData
                {
                    EventTitle = ev.Title,
                    GoingCount = going,
                    NotGoingCount = notGoing
                });
            }

            // 3. Get Detailed Member List for the Table
            AllMembers = await _context.Memberships
                .Where(m => m.SocietyId == socId)
                .Include(m => m.Student).ThenInclude(s => s.Person)
                .OrderBy(m => m.Student.Person.LastName)
                .ToListAsync();

            return Page();
        }

        // Handle CSV Download for Chairperson
        public IActionResult OnPostDownloadCsv()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = _context.Secretaries.FirstOrDefault(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Login");

            int socId = secretary.SocietyId.Value;

            var members = _context.Memberships
                .Where(m => m.SocietyId == socId)
                .Include(m => m.Student).ThenInclude(s => s.Person)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Student Number,First Name,Last Name,Email,Status,Request Date");

            foreach (var m in members)
            {
                sb.AppendLine($"{m.Student.StudentNumber},{m.Student.Person.FirstName},{m.Student.Person.LastName},{m.Student.Person.Email},{m.Status},{m.RequestDate:yyyy-MM-dd}");
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            string fileName = $"Society_Records_{SocietyName}_{DateTime.Now:yyyyMMdd}.csv";

            return File(fileBytes, "text/csv", fileName);
        }
    }
}