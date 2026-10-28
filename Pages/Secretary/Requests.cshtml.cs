using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Secretary
{
    public class RequestsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public RequestsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public IList<Membership> PendingRequests { get; set; } = new List<Membership>();
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

            // 2. Get all PENDING membership requests for this society
            // We include Student -> Person to get the student's name and number
            PendingRequests = await _context.Memberships
                .Where(m => m.SocietyId == societyId && m.Status == "PENDING")
                .Include(m => m.Student)
                    .ThenInclude(s => s.Person)
                .OrderByDescending(m => m.RequestDate)
                .ToListAsync();

            return Page();
        }

        // Handle Approve/Decline button clicks
        public async Task<IActionResult> OnPostAsync(int membershipId, string action)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var secretary = await _context.Secretaries.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (secretary == null || secretary.SocietyId == null) return RedirectToPage("/Login");

            // Find the specific membership request
            var membership = await _context.Memberships
                .Include(m => m.Student)
                .ThenInclude(s => s.Person)
                .FirstOrDefaultAsync(m => m.MembershipId == membershipId && m.SocietyId == secretary.SocietyId);

            if (membership == null)
            {
                ErrorMessage = "Request not found.";
                return await OnGetAsync();
            }

            // Update status based on button clicked
            if (action == "Approve")
            {
                membership.Status = "ACCEPTED";
                SuccessMessage = $"Approved {membership.Student.Person.FirstName} {membership.Student.Person.LastName}'s request.";
            }
            else if (action == "Decline")
            {
                membership.Status = "REJECTED";
                SuccessMessage = $"Declined {membership.Student.Person.FirstName} {membership.Student.Person.LastName}'s request.";
            }

            await _context.SaveChangesAsync();
            return await OnGetAsync(); // Refresh page to show updated list
        }
    }
}