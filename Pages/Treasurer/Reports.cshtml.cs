using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Treasurer
{
    public class ReportsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public ReportsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public string? SocietyName { get; set; }

        public decimal TotalBudget { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalContributions { get; set; }
        public decimal NetBalance { get; set; }

        [BindProperty] public string ReportTitle { get; set; } = string.Empty;
        [BindProperty] public string ReportMessage { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
        public IList<Announcement> SubmittedReports { get; set; } = new List<Announcement>();

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

            TotalBudget = await _context.Budgets.Where(b => b.SocietyId == socId).SumAsync(b => b.AmountAllocated ?? 0);
            TotalExpenses = await _context.Expenses.Where(e => e.SocietyId == socId).SumAsync(e => e.Amount ?? 0);
            TotalContributions = await _context.Contributions.Where(c => c.SocietyId == socId).SumAsync(c => c.Amount ?? 0);

            NetBalance = (TotalBudget + TotalContributions) - TotalExpenses;

            SubmittedReports = await _context.Announcements
                .Where(a => a.SocietyId == socId && a.Title.StartsWith("[TREASURER REPORT]"))
                .OrderByDescending(a => a.DatePosted)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            var treasurer = await _context.Treasurers.FirstOrDefaultAsync(t => t.PersonId == personId);

            if (treasurer == null || treasurer.SocietyId == null) return RedirectToPage();

            if (string.IsNullOrWhiteSpace(ReportTitle) || string.IsNullOrWhiteSpace(ReportMessage))
            {
                ModelState.AddModelError("", "Title and Message are required.");
                return await OnGetAsync();
            }

            var newReport = new Announcement
            {
                SocietyId = treasurer.SocietyId.Value,
                Title = $"[TREASURER REPORT] {ReportTitle}",
                Message = ReportMessage,
                DatePosted = DateTime.Now
            };

            _context.Announcements.Add(newReport);
            await _context.SaveChangesAsync();

            SuccessMessage = "Report submitted to Chairperson successfully!";
            ReportTitle = "";
            ReportMessage = "";

            return await OnGetAsync();
        }
    }
}