using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data;
using FinFinder.Web.Infrastructure.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Tests.Service.Tests
{
    [TestFixture]
    public class LikeServiceTests
    {
        private Mock<IRepository<Like, Guid>> _likeRepositoryMock;
        private LikeService _likeService;

        [SetUp]
        public void SetUp()
        {
            _likeRepositoryMock = new Mock<IRepository<Like, Guid>>();
            _likeService = new LikeService(_likeRepositoryMock.Object);
        }

        [Test]
        public async Task AddLikeAsync_Should_Add_Like_If_Not_Already_Exists()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingLikes = new List<Like>(); // No likes initially

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(existingLikes); // Ensure it supports async
            _likeRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            _likeRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Like>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var likeService = new LikeService(_likeRepositoryMock.Object);

            // Act
            var result = await likeService.AddLikeAsync(fishCatchId, userId);

            // Assert
            Assert.IsTrue(result);
            _likeRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Like>(
                l => l.FishCatchId == fishCatchId && l.UserId == userId)), Times.Once);
        }

        [Test]
        public async Task AddLikeAsync_Should_Return_False_If_Already_Exists()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var existingLikes = new List<Like>
    {
        new Like { Id = Guid.NewGuid(), FishCatchId = fishCatchId, UserId = userId }
    };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(existingLikes); // Ensure it supports async
            _likeRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            var likeService = new LikeService(_likeRepositoryMock.Object);

            // Act
            var result = await likeService.AddLikeAsync(fishCatchId, userId);

            // Assert
            Assert.IsFalse(result); // Should return false because the like already exists
            _likeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Like>()), Times.Never); // Ensure AddAsync is not called
        }


        [Test]
        public async Task RemoveLikeAsync_Should_Remove_Like_If_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();

            var likes = new List<Like>
    {
        new Like { Id = Guid.NewGuid(), UserId = userId, FishCatchId = fishCatchId }
    };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(likes);
            _likeRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            _likeRepositoryMock
                .Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var result = await _likeService.RemoveLikeAsync(fishCatchId, userId);

            // Assert
            Assert.IsTrue(result);
            _likeRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        }


        [Test]
        public async Task RemoveLikeAsync_Should_Return_False_If_Not_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();

            var likes = new List<Like>(); // No likes in the list
            var mockDbSet = DbSetMockHelper.CreateMockDbSet(likes); // Ensure mock supports async
            _likeRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _likeService.RemoveLikeAsync(fishCatchId, userId);

            // Assert
            Assert.IsFalse(result); // Since no like exists, it should return false
            _likeRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }


        [Test]
        public async Task GetLikedPostsAsync_Should_Return_All_Liked_Posts_For_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();

            var likedPosts = new List<Like>
    {
        new Like
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FishCatch = new FishCatch
            {
                Id = fishCatchId,
                Species = "Bass",
                LocationName = "Lake View",
                DateCaught = DateTime.UtcNow,
                Photos = new List<Photo>
                {
                    new Photo { Id = Guid.NewGuid(), Url = "/images/photo1.jpg" },
                    new Photo { Id = Guid.NewGuid(), Url = "/images/photo2.jpg" }
                },
                User = new ApplicationUser { UserName = "TestUser" }
            }
        }
    };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(likedPosts);
            _likeRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _likeService.GetLikedPostsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Bass", result.First().Species);
            Assert.AreEqual("Lake View", result.First().LocationName);
            Assert.AreEqual("TestUser", result.First().PublisherName);
            Assert.AreEqual(2, result.First().PhotoURLs.Count);
        }

    }
}
