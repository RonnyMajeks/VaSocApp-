using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public DashboardModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? FullName { get; set; }
        public string? UserType { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null)
            {
                return RedirectToPage("/Login");
            }

            FullName = HttpContext.Session.GetString("FullName");
            UserType = HttpContext.Session.GetString("UserType");

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);

            if (person != null)
            {
                // 1. Pass basic info to ViewData for the Layout dropdown
                ViewData["Username"] = person.Username;
                ViewData["Email"] = person.Email;

                // 2. Get University and Campus details
                if (person.UserType == "STUDENT")
                {
                    var student = await _context.Students
                        .Include(s => s.Faculty)
                        .FirstOrDefaultAsync(s => s.PersonId == personId);

                    if (student?.Faculty != null)
                    {
                        var university = await _context.Universities
                            .FirstOrDefaultAsync(u => u.Univid == student.Faculty.Univid);

                        ViewData["UniversityName"] = university?.Uniname;

                        var campus = await _context.Campuses
                            .FirstOrDefaultAsync(c => c.Univid == student.Faculty.Univid);

                        ViewData["CampusName"] = campus?.CampusName ?? "Main Campus";
                    }
                }
                else if (person.UserType == "STUDENT_AFFAIRS")
                {
                    var staff = await _context.StudentAffairs
                        .Include(s => s.Campus)
                        .FirstOrDefaultAsync(s => s.PersonId == personId);

                    if (staff != null)
                    {
                        if (staff.Univid.HasValue)
                        {
                            var university = await _context.Universities
                                .FirstOrDefaultAsync(u => u.Univid == staff.Univid);
                            ViewData["UniversityName"] = university?.Uniname;
                        }
                        ViewData["CampusName"] = staff.Campus?.CampusName;
                    }
                }
            }

            return Page();
        }
    }
}