using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentitySample.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var role = new ApplicationRole() { Name = Name };
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            return Page();

            }

            StatusMessage = $"Role '{Name}' has been created.";
            return RedirectToPage();
        }
    }
}