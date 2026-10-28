using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public IndexModel(VaSocDbContext context)
        {
            _context = context;
        }

        // Real data from database
        public int TotalStudents { get; set; }
        public int TotalSocieties { get; set; }
        public int TotalEvents { get; set; }
        public int TotalCampuses { get; set; }
        public int TotalAnnouncements { get; set; }

        public async Task OnGetAsync()
        {
            // Count real records from database
            TotalStudents = await _context.Students.CountAsync();
            TotalSocieties = await _context.Societies.CountAsync();
            TotalEvents = await _context.Events.CountAsync();
            TotalCampuses = await _context.Campuses.CountAsync();
            TotalAnnouncements = await _context.Announcements.CountAsync();
        }
    }
}