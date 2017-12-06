using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentitySample.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentitySample.Web.Pages.Admin
{
    public class EditRoleModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public EditRoleModel(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        [HiddenInput]
        public Guid Id { get; set; }

        [BindProperty]
        [Required]
        public string Name { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            Id = id;
            Name = role.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var role = await _roleManager.FindByIdAsync(Id.ToString());
            role.Name = Name;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            return Page();

            }

            StatusMessage = $"Role '{Name}' has been updated.";
            return RedirectToPage();
        }
    }
}