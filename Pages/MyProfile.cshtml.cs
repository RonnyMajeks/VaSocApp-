using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class MyProfileModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public MyProfileModel(VaSocDbContext context)
        {
            _context = context;
        }

        public Student? StudentRecord { get; set; }
        public Person? PersonRecord { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null)
            {
                return RedirectToPage("/Login");
            }

            // Get student with all navigation properties
            StudentRecord = await _context.Students
                .Include(s => s.Person)
                .Include(s => s.Faculty)
                .Include(s => s.Campus)
                .Include(s => s.CidNavigation)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (StudentRecord == null)
            {
                ErrorMessage = "Student profile not found. Please contact administration.";
                return Page();
            }

            PersonRecord = StudentRecord.Person;

            return Page();
        }
    }
}