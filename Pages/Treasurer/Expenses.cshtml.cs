using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Treasurer
{
    public class ExpensesModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public ExpensesModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }
        public decimal TotalExpenses { get; set; }
        public IList<Expense> ExpenseList { get; set; } = new List<Expense>();

        // Properties for the "Add Expense" form
        [BindProperty] public string Description { get; set; } = string.Empty;
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

            // Load expenses for this society
            ExpenseList = await _context.Expenses
                .Where(e => e.SocietyId == socId)
                .OrderByDescending(e => e.DateRecorded) // FIXED: Matches your model
                .ToListAsync();

            // Calculate total (handle nullable Amount)
            TotalExpenses = ExpenseList.Sum(e => e.Amount ?? 0);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            var treasurer = await _context.Treasurers.FirstOrDefaultAsync(t => t.PersonId == personId);

            if (treasurer == null || treasurer.SocietyId == null) return RedirectToPage();

            // Create new expense record
            var newExpense = new Expense
            {
                SocietyId = treasurer.SocietyId.Value,
                Description = Description,
                Amount = Amount,
                DateRecorded = DateOnly.FromDateTime(DateTime.Now) // FIXED: Matches your model
            };

            _context.Expenses.Add(newExpense);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Expense of R{Amount:N2} recorded successfully!";
            Description = "";
            Amount = 0;

            // Reload the page data
            return await OnGetAsync();
        }
    }
}