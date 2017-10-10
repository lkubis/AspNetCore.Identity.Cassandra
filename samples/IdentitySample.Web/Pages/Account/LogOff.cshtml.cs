using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentitySample.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentitySample.Web.Pages.Account
{
    public class LogOffModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogOffModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("./Login");
        }
    }
}