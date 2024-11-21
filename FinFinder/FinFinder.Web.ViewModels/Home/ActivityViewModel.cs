using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Home
{
    public class ActivityViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string ActionDescription { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

}
