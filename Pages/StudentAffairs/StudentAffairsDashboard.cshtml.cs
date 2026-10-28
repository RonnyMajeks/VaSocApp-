using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.StudentAffairs
{
    public class StudentAffairsDashboardModel : PageModel
    {
        private readonly VaSocDbContext _context;
        public StudentAffairsDashboardModel(VaSocDbContext context) { _context = context; }

        public string? FullName { get; set; }
        public int TotalSocieties { get; set; }
        public int ActiveMembers { get; set; }
        public decimal TotalBudget { get; set; }
        public int PendingRequests { get; set; }
        public IList<Announcement> RecentAnnouncements { get; set; } = new List<Announcement>();

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            FullName = HttpContext.Session.GetString("FullName");

            var staff = await _context.StudentAffairs
                .Include(s => s.Campus)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (staff != null)
            {
                ViewData["UniversityName"] = staff.Univid.HasValue
                    ? (await _context.Universities.FirstOrDefaultAsync(u => u.Univid == staff.Univid))?.Uniname
                    : null;
                ViewData["CampusName"] = staff.Campus?.CampusName;

                if (staff.CampusId.HasValue)
                {
                    int campusId = staff.CampusId.Value;
                    var societyIds = await _context.Societies
                        .Where(s => s.CampusId == campusId)
                        .Select(s => s.SocietyId)
                        .ToListAsync();

                    TotalSocieties = societyIds.Count;
                    ActiveMembers = await _context.Memberships
                        .Where(m => societyIds.Contains(m.SocietyId ?? 0) && m.Status == "ACCEPTED")
                        .CountAsync();
                    TotalBudget = await _context.Budgets
                        .Where(b => societyIds.Contains(b.SocietyId ?? 0))
                        .SumAsync(b => (decimal?)b.AmountAllocated) ?? 0;
                    PendingRequests = await _context.Memberships
                        .Where(m => societyIds.Contains(m.SocietyId ?? 0) && m.Status == "PENDING")
                        .CountAsync();

                    RecentAnnouncements = await _context.Announcements
                        .Where(a => a.Title.StartsWith("[CAMPUS NOTICE]"))
                        .OrderByDescending(a => a.DatePosted)
                        .Take(5)
                        .ToListAsync();
                }
            }

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            if (person != null)
            {
                ViewData["Username"] = person.Username;
                ViewData["Email"] = person.Email;
            }

            return Page();
        }
    }
}