using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;
using System.Text.Json;

namespace VaSocApp_System.Pages.Chairperson
{
    public class AnalyticsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public AnalyticsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }

       
        public int TotalMembers { get; set; }
        public int PendingRequests { get; set; }
        public int TotalEvents { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalAnnouncements { get; set; }
        public double AcceptanceRate { get; set; }
        public double ActivityScore { get; set; }

        
        public string YearChartData { get; set; } = "[]";
        public string CultureChartData { get; set; } = "[]";
        public string ActivityChartData { get; set; } = "[]";

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == personId);
            if (chairperson == null || chairperson.SocietyId == null) return Page();

            var society = await _context.Societies.FirstOrDefaultAsync(s => s.SocietyId == chairperson.SocietyId);
            SocietyName = society?.SocietyName;
            int socId = chairperson.SocietyId.Value;

          
            TotalMembers = await _context.Memberships.CountAsync(m => m.SocietyId == socId && m.Status == "ACCEPTED");
            PendingRequests = await _context.Memberships.CountAsync(m => m.SocietyId == socId && m.Status == "PENDING");
            TotalEvents = await _context.Events.CountAsync(e => e.SocietyId == socId);
            TotalMeetings = await _context.Meetings.CountAsync(m => m.SocietyId == socId);
            TotalAnnouncements = await _context.Announcements.CountAsync(a => a.SocietyId == socId);

           
            int totalRequests = TotalMembers + PendingRequests;
            AcceptanceRate = totalRequests > 0 ? Math.Round((double)TotalMembers / totalRequests * 100, 1) : 0;

           
            ActivityScore = (TotalEvents * 3) + (TotalMeetings * 2) + TotalAnnouncements;

           
            var yearData = await (from m in _context.Memberships
                                  join s in _context.Students on m.StudentId equals s.StudentId
                                  where m.SocietyId == socId && m.Status == "ACCEPTED" && s.YearOfStudy != null
                                  group m by s.YearOfStudy into g
                                  orderby g.Key
                                  select new { label = "Year " + g.Key, value = g.Count() })
                .ToListAsync();
            YearChartData = JsonSerializer.Serialize(yearData);

            
            var cultureData = await (from m in _context.Memberships
                                     join s in _context.Students on m.StudentId equals s.StudentId
                                     join c in _context.Cultures on s.Cid equals c.Cid into cultureGroup
                                     from c in cultureGroup.DefaultIfEmpty()
                                     where m.SocietyId == socId && m.Status == "ACCEPTED"
                                     group m by (c == null ? "Unknown" : c.CultureName) into g
                                     orderby g.Count() descending
                                     select new { label = g.Key, value = g.Count() })
                .ToListAsync();
            CultureChartData = JsonSerializer.Serialize(cultureData);

           
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var activityData = new List<object>();

            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var eventsCount = await _context.Events.CountAsync(e => e.SocietyId == socId && e.EventDate >= DateOnly.FromDateTime(monthStart) && e.EventDate < DateOnly.FromDateTime(monthEnd));
                var meetingsCount = await _context.Meetings.CountAsync(m => m.SocietyId == socId && m.Schedule >= DateOnly.FromDateTime(monthStart) && m.Schedule < DateOnly.FromDateTime(monthEnd));
                var announcementsCount = await _context.Announcements.CountAsync(a => a.SocietyId == socId && a.DatePosted >= monthStart && a.DatePosted < monthEnd);

                activityData.Add(new { label = month.ToString("MMM yyyy"), events = eventsCount, meetings = meetingsCount, announcements = announcementsCount });
            }
            ActivityChartData = JsonSerializer.Serialize(activityData);

            return Page();
        }
    }
}