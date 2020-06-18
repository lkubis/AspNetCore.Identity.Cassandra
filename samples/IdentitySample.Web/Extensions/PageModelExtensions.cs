using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentitySample.Web.Extensions
{
    public static class PageModelExtensions
    {
        public static void AddIdentityErrors(this PageModel pageModel, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                pageModel.ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
