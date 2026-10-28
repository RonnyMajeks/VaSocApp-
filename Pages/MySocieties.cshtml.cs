using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class MySocietiesModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public MySocietiesModel(VaSocDbContext context)
        {
            _context = context;
        }

        // List to hold the societies the student has successfully joined
        public IList<Membership> MyMemberships { get; set; } = new List<Membership>();

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Check if user is logged in
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // 2. Get the student record to find their StudentId
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (student == null) return Page();

            // 3. Fetch only ACCEPTED memberships and include the Society details
            MyMemberships = await _context.Memberships
                .Where(m => m.StudentId == student.StudentId && m.Status == "ACCEPTED")
                .Include(m => m.Society)
                .ToListAsync();

            return Page();
        }
    }
}