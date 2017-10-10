using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentitySample.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace IdentitySample.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ISession _session;

        public IndexModel(ISession session)
        {
            _session = session;
        }

        public List<ApplicationUser> Users { get; private set; }

        public async Task OnGetAsync()
        {
            var mapper = new Mapper(_session);
            Users = (await mapper.FetchAsync<ApplicationUser>()).ToList();
        }
    }
}