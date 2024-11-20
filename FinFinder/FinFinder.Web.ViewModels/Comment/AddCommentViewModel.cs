using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Comment
{
    public class AddCommentViewModel
    {
        public Guid FishCatchId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

}
