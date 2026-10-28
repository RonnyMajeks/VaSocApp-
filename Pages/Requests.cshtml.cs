using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class RequestsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public RequestsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Membership> MyRequests { get; set; } = new List<Membership>();

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (student == null) return Page();

            // Get all memberships for this student, including the Society name
            MyRequests = await _context.Memberships
                .Where(m => m.StudentId == student.StudentId)
                .Include(m => m.Society)
                .OrderByDescending(m => m.RequestDate)
                .ToListAsync();

            return Page();
        }
    }
}