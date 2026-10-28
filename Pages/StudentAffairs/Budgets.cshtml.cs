using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.StudentAffairs
{
    public class BudgetsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public BudgetsModel(VaSocDbContext context)
        {
            _context = context;
        }

        // Display Data
        public IList<Budget> BudgetsList { get; set; } = new List<Budget>();
        public IList<Society> SocietiesList { get; set; } = new List<Society>();
        public decimal TotalAllocated { get; set; }

        // Form properties for allocating budget
        [BindProperty] public int? SocietyId { get; set; }
        [BindProperty] public decimal AmountAllocated { get; set; }
        [BindProperty] public string? AcademicYear { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // Get the staff member's assigned campus
            var staff = await _context.StudentAffairs.FirstOrDefaultAsync(s => s.PersonId == personId);

            if (staff == null || staff.CampusId == null)
            {
                ErrorMessage = "Your account is not assigned to a campus.";
                return Page();
            }

            int myCampusId = staff.CampusId.Value;

            // STRICT filter: Only show societies on THIS campus
            SocietiesList = await _context.Societies
                .Where(s => s.CampusId == myCampusId)
                .OrderBy(s => s.SocietyName)
                .ToListAsync();

            // Load budget allocations for THIS campus only
            var campusSocietyIds = SocietiesList.Select(s => s.SocietyId).ToList();

            BudgetsList = await _context.Budgets
                .Include(b => b.Society)
                .Where(b => campusSocietyIds.Contains(b.SocietyId ?? 0))
                .OrderByDescending(b => b.DateAllocated)
                .ToListAsync();

            // Calculate total allocated across campus societies only
            TotalAllocated = BudgetsList.Sum(b => b.AmountAllocated ?? 0);

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

            // Validation
            if (SocietyId == null || SocietyId.Value <= 0)
            {
                ErrorMessage = "Please select a society.";
                return await OnGetAsync();
            }

            // Verify the selected society belongs to this campus
            var society = await _context.Societies.FirstOrDefaultAsync(s => s.SocietyId == SocietyId && s.CampusId == myCampusId);
            if (society == null)
            {
                ErrorMessage = "Invalid society selection.";
                return await OnGetAsync();
            }

            if (AmountAllocated <= 0)
            {
                ErrorMessage = "Amount must be greater than zero.";
                return await OnGetAsync();
            }

            if (string.IsNullOrWhiteSpace(AcademicYear))
            {
                ErrorMessage = "Academic year is required.";
                return await OnGetAsync();
            }

            try
            {
                var budget = new Budget
                {
                    SocietyId = SocietyId.Value,
                    AmountAllocated = AmountAllocated,
                    AcademicYear = AcademicYear,
                    DateAllocated = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();

                SuccessMessage = $"Budget of R{AmountAllocated:N2} allocated successfully to {society.SocietyName} for {AcademicYear}.";

                // Clear form
                SocietyId = null;
                AmountAllocated = 0;
                AcademicYear = "";

                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error allocating budget: " + ex.Message;
                return await OnGetAsync();
            }
        }
    }
}