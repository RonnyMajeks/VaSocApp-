using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.StudentAffairs
{
    public class AnnouncementsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public AnnouncementsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Announcement> AnnouncementsList { get; set; } = new List<Announcement>();

        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public string Message { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var staff = await _context.StudentAffairs.FirstOrDefaultAsync(s => s.PersonId == personId);

            if (staff == null || staff.CampusId == null)
            {
                ErrorMessage = "Your account is not assigned to a campus.";
                return Page();
            }

            int myCampusId = staff.CampusId.Value;

            AnnouncementsList = await _context.Announcements
                .Include(a => a.Society)
                .Where(a => a.Title.StartsWith("[CAMPUS NOTICE]"))
                .OrderByDescending(a => a.DatePosted)
                .Take(50)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            var staff = await _context.StudentAffairs.FirstOrDefaultAsync(s => s.PersonId == personId);

            if (staff == null || staff.CampusId == null)
            {
                ErrorMessage = "Your account is not assigned to a campus.";
                return await OnGetAsync();
            }

            int myCampusId = staff.CampusId.Value;

            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Message))
            {
                ErrorMessage = "Title and Message are required.";
                return await OnGetAsync();
            }

            try
            {
                string finalTitle = $"[CAMPUS NOTICE] {Title}";
                DateTime now = DateTime.Now;

                var campusSocieties = await _context.Societies
                    .Where(s => s.CampusId == myCampusId)
                    .ToListAsync();

                foreach (var society in campusSocieties)
                {
                    _context.Announcements.Add(new Announcement
                    {
                        SocietyId = society.SocietyId,
                        Title = finalTitle,
                        Message = Message,
                        DatePosted = now
                    });
                }

                await _context.SaveChangesAsync();

                SuccessMessage = "Campus notice broadcasted to all societies successfully!";

                Title = "";
                Message = "";

                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error sending announcement: " + ex.Message;
                return await OnGetAsync();
            }
        }
    }
}