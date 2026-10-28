using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VaSocApp_System.Pages.Treasurer
{
    public class ContributionsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public ContributionsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }
        public decimal TotalContributions { get; set; }
        public IList<Contribution> ContributionList { get; set; } = new List<Contribution>();

        // Dropdown for selecting who paid
        public SelectList PersonSelectList { get; set; } = new SelectList(new List<object>());

        // Form properties
        [BindProperty] public int? PersonId { get; set; }
        [BindProperty] public decimal Amount { get; set; }

        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var treasurer = await _context.Treasurers
                .Include(t => t.Society)
                .FirstOrDefaultAsync(t => t.PersonId == personId);

            if (treasurer == null || treasurer.SocietyId == null)
            {
                SocietyName = "No Society Assigned";
                return Page();
            }

            SocietyName = treasurer.Society?.SocietyName ?? "Unknown Society";
            int? socId = treasurer.SocietyId;

            // Load contributions for this society
            ContributionList = await _context.Contributions
                .Where(c => c.SocietyId == socId)
                .Include(c => c.Person) // Load person details to show name
                .OrderByDescending(c => c.ContiId)
                .ToListAsync();

            TotalContributions = ContributionList.Sum(c => c.Amount ?? 0);

            // Load list of people for the dropdown (Filtering for Students makes it cleaner)
            var people = await _context.Persons
                .Where(p => p.UserType == "STUDENT")
                .Select(p => new { p.PersonId, DisplayName = $"{p.FirstName} {p.LastName} ({p.Username})" })
                .ToListAsync();

            PersonSelectList = new SelectList(people, "PersonId", "DisplayName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            var treasurer = await _context.Treasurers.FirstOrDefaultAsync(t => t.PersonId == personId);

            if (treasurer == null || treasurer.SocietyId == null) return RedirectToPage();

            if (PersonId == null)
            {
                ModelState.AddModelError("PersonId", "Please select a person.");
                return await OnGetAsync(); // Reload to show error
            }

            var newContribution = new Contribution
            {
                SocietyId = treasurer.SocietyId.Value,
                PersonId = PersonId.Value,
                Amount = Amount
            };

            _context.Contributions.Add(newContribution);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Contribution of R{Amount:N2} recorded successfully!";
            PersonId = null;
            Amount = 0;

            return await OnGetAsync();
        }
    }
}