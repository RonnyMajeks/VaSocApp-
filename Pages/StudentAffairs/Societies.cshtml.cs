using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages.StudentAffairs
{
    public class SocietiesModel : PageModel
    {
        private readonly VaSocDbContext _context;
        public SocietiesModel(VaSocDbContext context) { _context = context; }

        public IList<Society> SocietiesList { get; set; } = new List<Society>();
        public IList<Campus> CampusesList { get; set; } = new List<Campus>();

        // Property to hold the campus name for display in Step 3
        public string? SelectedCampusName { get; set; }

        [BindProperty] public int CurrentStep { get; set; } = 1;
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        [BindProperty] public string SocietyName { get; set; } = string.Empty;
        [BindProperty] public string? Description { get; set; }
        [BindProperty] public int? CampusId { get; set; }

        [BindProperty] public string ChairFirstName { get; set; } = string.Empty;
        [BindProperty] public string ChairLastName { get; set; } = string.Empty;
        [BindProperty] public string ChairEmail { get; set; } = string.Empty;
        [BindProperty] public string ChairUsername { get; set; } = string.Empty;

        [BindProperty] public string SecFirstName { get; set; } = string.Empty;
        [BindProperty] public string SecLastName { get; set; } = string.Empty;
        [BindProperty] public string SecEmail { get; set; } = string.Empty;
        [BindProperty] public string SecUsername { get; set; } = string.Empty;

        [BindProperty] public string TreasFirstName { get; set; } = string.Empty;
        [BindProperty] public string TreasLastName { get; set; } = string.Empty;
        [BindProperty] public string TreasEmail { get; set; } = string.Empty;
        [BindProperty] public string TreasUsername { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            await LoadDataAsync(personId.Value);
            CurrentStep = 1;
            return Page();
        }

        public IActionResult OnPostNextAsync()
        {
            // CRITICAL FIX: Remove CurrentStep from ModelState so the view uses the updated value
            ModelState.Remove("CurrentStep");

            if (CurrentStep == 1)
            {
                if (string.IsNullOrWhiteSpace(SocietyName))
                {
                    ErrorMessage = "Society name is required.";
                    return Page();
                }

                // Store the campus name when moving from Step 1
                if (CampusId.HasValue)
                {
                    var campus = CampusesList.FirstOrDefault(c => c.CampusId == CampusId.Value);
                    SelectedCampusName = campus?.CampusName ?? "Unknown Campus";
                }
            }

            CurrentStep++;
            return Page();
        }

        public IActionResult OnPostBackAsync()
        {
            // CRITICAL FIX: Remove CurrentStep from ModelState
            ModelState.Remove("CurrentStep");

            if (CurrentStep > 1) CurrentStep--;
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var staff = await _context.StudentAffairs.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (staff == null || staff.CampusId == null)
            {
                ErrorMessage = "Your account is not assigned to a campus.";
                return Page();
            }

            CampusId = staff.CampusId.Value;

            try
            {
                var society = new Society { SocietyName = SocietyName, Description = Description, CampusId = CampusId };
                _context.Societies.Add(society);
                await _context.SaveChangesAsync();
                int societyId = society.SocietyId;

                string defaultHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

                var chairPerson = new Person { FirstName = ChairFirstName, LastName = ChairLastName, Email = ChairEmail, Username = ChairUsername, PasswordHash = defaultHash, UserType = "SOCIETY_LEADER", IsActive = true, CreatedAt = DateTime.Now };
                _context.Persons.Add(chairPerson);
                await _context.SaveChangesAsync();
                _context.Chairpersons.Add(new VaSocApp_System.Models.Chairperson { PersonId = chairPerson.PersonId, SocietyId = societyId });

                var secPerson = new Person { FirstName = SecFirstName, LastName = SecLastName, Email = SecEmail, Username = SecUsername, PasswordHash = defaultHash, UserType = "SOCIETY_LEADER", IsActive = true, CreatedAt = DateTime.Now };
                _context.Persons.Add(secPerson);
                await _context.SaveChangesAsync();
                _context.Secretaries.Add(new VaSocApp_System.Models.Secretary { PersonId = secPerson.PersonId, SocietyId = societyId });

                var treasPerson = new Person { FirstName = TreasFirstName, LastName = TreasLastName, Email = TreasEmail, Username = TreasUsername, PasswordHash = defaultHash, UserType = "SOCIETY_LEADER", IsActive = true, CreatedAt = DateTime.Now };
                _context.Persons.Add(treasPerson);
                await _context.SaveChangesAsync();
                _context.Treasurers.Add(new VaSocApp_System.Models.Treasurer { PersonId = treasPerson.PersonId, SocietyId = societyId });

                await _context.SaveChangesAsync();

                SuccessMessage = $"Society '{SocietyName}' registered successfully!";

                ModelState.Clear();
                ResetForm();
                CurrentStep = 1;

                await LoadDataAsync(personId.Value);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
                return Page();
            }
        }

        private async Task LoadDataAsync(int personId)
        {
            var staff = await _context.StudentAffairs.Include(s => s.Campus).FirstOrDefaultAsync(s => s.PersonId == personId);
            if (staff != null && staff.CampusId.HasValue)
            {
                int myCampusId = staff.CampusId.Value;
                SocietiesList = await _context.Societies.Include(s => s.Campus).Where(s => s.CampusId == myCampusId).OrderBy(s => s.SocietyName).ToListAsync();
                CampusesList = await _context.Campuses.Where(c => c.CampusId == myCampusId).OrderBy(c => c.CampusName).ToListAsync();
                if (!CampusId.HasValue) CampusId = myCampusId;

                SelectedCampusName = staff.Campus?.CampusName ?? "Unknown Campus";
            }
        }

        private void ResetForm()
        {
            SocietyName = ""; Description = ""; CampusId = null; SelectedCampusName = null;
            ChairFirstName = ""; ChairLastName = ""; ChairEmail = ""; ChairUsername = "";
            SecFirstName = ""; SecLastName = ""; SecEmail = ""; SecUsername = "";
            TreasFirstName = ""; TreasLastName = ""; TreasEmail = ""; TreasUsername = "";
            SuccessMessage = null; ErrorMessage = null;
        }
    }
}