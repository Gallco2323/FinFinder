using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Like
{
    public class LikeViewModel
    {
        public Guid FishCatchId { get; set; }
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

}
