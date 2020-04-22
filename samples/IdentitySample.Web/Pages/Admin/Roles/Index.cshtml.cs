using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Cassandra.Extensions;
using Cassandra.Data.Linq;
using IdentitySample.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentitySample.Web.Pages.Admin
{
    public class RolesModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesModel(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public List<ApplicationRole> Roles { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            Roles = (await _roleManager.Roles.AsCqlQuery().ExecuteAsync()).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            StatusMessage = $"Role '{role.Name}' hass been deleted.";
            return RedirectToPage("./Index");
        }
    }
}