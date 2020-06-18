using AspNetCore.Identity.Cassandra.Extensions;
using IdentitySample.Web.Data;
using IdentitySample.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            IdentityResult result = null;

            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var c in claims)
            {
                result = await _roleManager.RemoveClaimAsync(role, c);
                if (!result.Succeeded)
                {
                    this.AddIdentityErrors(result);
                    return Page();
                }
            }

            result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                this.AddIdentityErrors(result);
                return Page();
            }

            StatusMessage = $"Role '{role.Name}' has been deleted.";
            return RedirectToPage("./Index");
        }
    }
}