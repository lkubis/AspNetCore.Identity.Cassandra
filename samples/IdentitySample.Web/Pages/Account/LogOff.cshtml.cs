using System.Threading.Tasks;
using IdentitySample.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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