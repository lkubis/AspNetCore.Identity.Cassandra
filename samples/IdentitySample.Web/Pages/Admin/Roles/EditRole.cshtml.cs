using IdentitySample.Web.Data;
using IdentitySample.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

        [Display(Name = "Claims (comma delimited)")]
        [BindProperty]
        public string Claims { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            Id = id;
            Name = role.Name;

            Claims = string.Join(", ", (await _roleManager.GetClaimsAsync(role)).Select(x => x.Value));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var role = await _roleManager.FindByIdAsync(Id.ToString());
            role.Name = Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var existingClaims = (await _roleManager.GetClaimsAsync(role)).Select(x => x.Value).ToList();
                var currentClaims = Claims.Split(",").Select(x => x.Trim()).ToList();
                foreach (var c in existingClaims.Except(currentClaims))
                {
                    result = await _roleManager.RemoveClaimAsync(role, new System.Security.Claims.Claim("custom", c));
                    if (!result.Succeeded)
                    {
                        this.AddIdentityErrors(result);
                        return Page();
                    }
                }

                foreach (var c in currentClaims.Except(existingClaims))
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

            StatusMessage = $"Role '{Name}' has been updated.";
            return RedirectToPage();
        }
    }
}