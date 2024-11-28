using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data
{
    public class LikeService : ILikeService
    {
        private readonly IRepository<Like, Guid> _likeRepository;

        public LikeService(IRepository<Like,Guid> likeRepository)
        {
                _likeRepository = likeRepository;
        }
        public async Task<bool> AddLikeAsync(Guid fishCatchId, Guid userId)
        {
            var existingLike = await _likeRepository.GetAllAttached()
            .FirstOrDefaultAsync(l => l.FishCatchId == fishCatchId && l.UserId == userId);

            if (existingLike != null)
            {
                return false; // Like already exists
            }

            // Add the new like
            var like = new Like
            {
                Id = Guid.NewGuid(),
                FishCatchId = fishCatchId,
                UserId = userId
            };

            await _likeRepository.AddAsync(like);
            return true;
        }

        public async Task<bool> RemoveLikeAsync(Guid fishCatchId, Guid userId)
        {
            var like = await _likeRepository.GetAllAttached()
                .FirstOrDefaultAsync(l => l.FishCatchId == fishCatchId && l.UserId == userId);
            

            if (like == null)
            {
                return false; // Like does not exist
            }

            // Remove the like
            await _likeRepository.DeleteAsync(like.Id);
            return true;
        }
    }
}
