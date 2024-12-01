using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Admin
{
    public class EditUserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
    }
}
