using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VaSocApp_System.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Redirect to login page
            return RedirectToPage("/Login");
        }
    }
}