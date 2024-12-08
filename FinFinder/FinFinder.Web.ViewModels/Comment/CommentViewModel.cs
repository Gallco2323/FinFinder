using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Comment
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string FishCatchId { get; set; } = string.Empty;
        // Name of the user who made the comment
        public DateTime DateCreated { get; set; } // When the comment was created
    }

}
