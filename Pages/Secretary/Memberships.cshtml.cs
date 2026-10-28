using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class MembershipsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public MembershipsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Membership> MembersList { get; set; } = new List<Membership>();
        public string? SocietyName { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // 1. Get the society this secretary belongs to
            var secretary = await _context.Secretaries
                .Include(s => s.Society)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (secretary == null || secretary.SocietyId == null)
            {
                ErrorMessage = "You are not assigned to a society.";
                return Page();
            }

            SocietyName = secretary.Society.SocietyName;
            int societyId = secretary.SocietyId.Value;

            // 2. Get all ACCEPTED or SUSPENDED members for this society
            MembersList = await _context.Memberships
                .Where(m => m.SocietyId == societyId && (m.Status == "ACCEPTED" || m.Status == "SUSPENDED"))
                .Include(m => m.Student)
                    .ThenInclude(s => s.Person)
                .OrderBy(m => m.Student.Person.LastName)
                .ToListAsync();

            return Page();
        }

        // Handle Suspend, Reinstate, or Remove button clicks
        public async Task<IActionResult> OnPostAsync(int membershipId, string action)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Login");

            // Find the specific membership
            var membership = await _context.Memberships
                .Include(m => m.Student)
                .ThenInclude(s => s.Person)
                .FirstOrDefaultAsync(m => m.MembershipId == membershipId && m.SocietyId == secretary.SocietyId);

            if (membership == null)
            {
                ErrorMessage = "Member not found.";
                return await OnGetAsync();
            }

            string studentName = $"{membership.Student.Person.FirstName} {membership.Student.Person.LastName}";

            if (action == "Suspend")
            {
                membership.Status = "SUSPENDED";
                SuccessMessage = $"{studentName} has been suspended.";
            }
            else if (action == "Reinstate")
            {
                membership.Status = "ACCEPTED";
                SuccessMessage = $"{studentName} has been reinstated as an active member.";
            }
            else if (action == "Remove")
            {
                _context.Memberships.Remove(membership);
                SuccessMessage = $"{studentName} has been removed from the society.";
            }

            await _context.SaveChangesAsync();
            return await OnGetAsync(); // Refresh page to show updated list
        }
    }
}