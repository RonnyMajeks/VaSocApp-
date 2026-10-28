using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public RegisterModel(VaSocDbContext context)
        {
            _context = context;
        }

        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [BindProperty]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Student number is required.")]
        [RegularExpression(@"^\d{9,10}$", ErrorMessage = "Student number must be 9 or 10 digits.")]
        [BindProperty]
        public string StudentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a university.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid university selection.")]
        [BindProperty]
        public int UniversityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid campus selection.")]
        [BindProperty]
        public int? CampusId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid faculty selection.")]
        [BindProperty]
        public int? FacultyId { get; set; }

        [Range(1, 4, ErrorMessage = "Please select a valid year of study.")]
        [BindProperty]
        public int? YearOfStudy { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a culture.")]
        [BindProperty]
        public int? CultureId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<University> Universities { get; set; } = new List<University>();
        public List<Campus> Campuses { get; set; } = new List<Campus>();
        public List<Faculty> Faculties { get; set; } = new List<Faculty>();
        public List<Culture> Cultures { get; set; } = new List<Culture>();

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? universityId = null)
        {
            Universities = await _context.Universities.OrderBy(u => u.Uniname).ToListAsync();
            Cultures = await _context.Cultures.OrderBy(c => c.CultureName).ToListAsync();
            Faculties = await _context.Faculties.OrderBy(f => f.FacultyName).ToListAsync();

            if (universityId.HasValue && universityId.Value > 0)
            {
                Campuses = await _context.Campuses
                    .Where(c => c.Univid == universityId.Value)
                    .OrderBy(c => c.CampusName)
                    .ToListAsync();
                UniversityId = universityId.Value;
            }
            else
            {
                Campuses = await _context.Campuses.OrderBy(c => c.CampusName).ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(UniversityId);
            }

            bool studentExists = await _context.Students.AnyAsync(s => s.StudentNumber == StudentNumber);
            if (studentExists)
            {
                ModelState.AddModelError("StudentNumber", "A student with this number is already registered.");
                return await OnGetAsync(UniversityId);
            }

            var university = await _context.Universities.FindAsync(UniversityId);
            if (university == null || string.IsNullOrEmpty(university.StudentDomain))
            {
                ModelState.AddModelError("UniversityId", "Invalid university selected.");
                return await OnGetAsync(UniversityId);
            }

            string generatedEmail = $"{StudentNumber}{university.StudentDomain}";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            try
            {
                var person = new Person
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = generatedEmail,
                    Phone = Phone,
                    Username = StudentNumber,
                    PasswordHash = hashedPassword,
                    UserType = "STUDENT",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                var student = new Student
                {
                    PersonId = person.PersonId,
                    StudentNumber = StudentNumber,
                    FacultyId = FacultyId,
                    YearOfStudy = YearOfStudy,
                    Cid = CultureId
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                SuccessMessage = "Registration successful! You can now log in with your Student Number.";

                FirstName = "";
                LastName = "";
                Phone = "";
                StudentNumber = "";
                Password = "";
                ConfirmPassword = "";

                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Registration failed: " + ex.Message;
                return await OnGetAsync(UniversityId);
            }
        }
    }
}