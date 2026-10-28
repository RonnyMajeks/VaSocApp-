using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.Chairperson
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

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var chairperson = await _context.Chairpersons.FirstOrDefaultAsync(c => c.PersonId == personId);
            if (chairperson == null || chairperson.SocietyId == null) return Page();

            var society = await _context.Societies.FirstOrDefaultAsync(s => s.SocietyId == chairperson.SocietyId);
            SocietyName = society?.SocietyName;

            MembersList = await _context.Memberships
                .Where(m => m.SocietyId == chairperson.SocietyId)
                .Include(m => m.Student).ThenInclude(s => s.Person)
                .OrderBy(m => m.Status)
                .ToListAsync();

            return Page();
        }
    }
}