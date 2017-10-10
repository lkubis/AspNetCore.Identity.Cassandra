using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentitySample.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentitySample.Web.Pages
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public Guid UserId { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Email { get; set; }

        public EditModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return RedirectToPage("/Index");

            UserId = id;
            Email = user.Email;
            Phone = user.Phone?.Number;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByIdAsync(UserId.ToString());
            var result = await _userManager.SetPhoneNumberAsync(user, Phone);
            return RedirectToPage("/Index");
        }
    }
}