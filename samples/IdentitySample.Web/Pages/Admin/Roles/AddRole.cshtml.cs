using IdentitySample.Web.Data;
using IdentitySample.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace IdentitySample.Web.Pages.Admin
{
    public class AddRoleModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AddRoleModel(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Claims (comma delimited)")]
        [BindProperty]
        public string Claims { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var role = new ApplicationRole() { Name = Name };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded && !string.IsNullOrEmpty(Claims))
            {
                foreach (var c in Claims.Split(","))
                {
                    result = await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("custom", c));
                    if (!result.Succeeded)
                    {
                        this.AddIdentityErrors(result);
                        return Page();
                    }
                }
            }

            if (!result.Succeeded)
            {
                this.AddIdentityErrors(result);
                return Page();
            }

            StatusMessage = $"Role '{Name}' has been created.";
            return RedirectToPage();
        }
    }
}