using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

namespace VaSocApp_System.Pages
{
    public class SocietyDetailsModel : PageModel
    {
        private readonly VaSocDbContext _context;

        public SocietyDetailsModel(VaSocDbContext context)
        {
            _context = context;
        }

        public Society? SocietyInfo { get; set; }
        public string? CampusName { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public bool AlreadyRequested { get; set; }
        public bool AlreadyMember { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            // Get society details
            SocietyInfo = await _context.Societies
                .Include(s => s.Campus)
                .FirstOrDefaultAsync(s => s.SocietyId == id);

            if (SocietyInfo == null) return NotFound();

            CampusName = SocietyInfo.Campus?.CampusName ?? "Unknown";

            // Check if user already requested or is a member
            var student = await _context.Students.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (student != null)
            {
                var existingMembership = await _context.Memberships
                    .FirstOrDefaultAsync(m => m.StudentId == student.StudentId && m.SocietyId == id);

                if (existingMembership != null)
                {
                    if (existingMembership.Status == "ACCEPTED")
                    {
                        AlreadyMember = true;
                    }
                    else
                    {
                        AlreadyRequested = true;
                    }
                }
            }

            return Page();
        }

        // Handle the "Request to Join" button
        public async Task<IActionResult> OnPostRequestAsync(int societyId)
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.PersonId == personId);
            if (student == null)
            {
                ErrorMessage = "Student record not found.";
                return await OnGetAsync(societyId);
            }

            // Check for duplicate
            var existingMembership = await _context.Memberships
                .FirstOrDefaultAsync(m => m.StudentId == student.StudentId && m.SocietyId == societyId);

            if (existingMembership != null)
            {
                if (existingMembership.Status == "ACCEPTED")
                {
                    ErrorMessage = "You are already a member of this society!";
                }
                else
                {
                    ErrorMessage = "You have already requested to join this society. Please wait for approval.";
                }
                return await OnGetAsync(societyId);
            }

            // Create new membership request
            var newMembership = new Membership
            {
                StudentId = student.StudentId,
                SocietyId = societyId,
                Status = "PENDING",
                RequestDate = DateTime.UtcNow
            };

            _context.Memberships.Add(newMembership);
            await _context.SaveChangesAsync();

            SuccessMessage = "Your request has been submitted successfully! You will be notified once approved.";
            return await OnGetAsync(societyId);
        }
    }
}