using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentitySample.Web.Pages.Admin.Roles
{
    public static class RolesNavPages
    {
        public static string Index => "Index";

        public static string AddRole => "AddRole";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string AddRoleNavClass(ViewContext viewContext) => PageNavClass(viewContext, AddRole);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
