using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Treasurer
{
    public class BudgetModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public BudgetModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }
        public decimal TotalAllocated { get; set; }
        public IList<Budget> BudgetList { get; set; } = new List<Budget>();

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // Find the treasurer and their society
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

            // Get all budget records for this society
            BudgetList = await _context.Budgets
                .Where(b => b.SocietyId == socId)
                .OrderByDescending(b => b.AcademicYear)
                .ToListAsync();

            // Calculate total allocated amount (handle nullable)
            TotalAllocated = BudgetList.Sum(b => b.AmountAllocated ?? 0);

            return Page();
        }
    }
}