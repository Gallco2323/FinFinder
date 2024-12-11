using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
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
    public class FavoriteServiceTests
    {
        private Mock<ICompositeKeyRepository<Favorite, Guid, Guid>> _favoriteRepositoryMock;
        private Mock<IRepository<FishCatch, Guid>> _fishCatchRepositoryMock;
        private IFavoriteService _service;

        [SetUp]
        public void SetUp()
        {
            _favoriteRepositoryMock = new Mock<ICompositeKeyRepository<Favorite, Guid, Guid>>();
            _fishCatchRepositoryMock = new Mock<IRepository<FishCatch, Guid>>();
            _service = new FavoriteService(_favoriteRepositoryMock.Object, _fishCatchRepositoryMock.Object);
        }

        [Test]
        public async Task GetUserFavoritesAsync_Should_Return_Favorites_For_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();

            var favorites = new List<Favorite>
            {
                new Favorite
                {
                    UserId = userId,
                    FishCatchId = fishCatchId,
                    FishCatch = new FishCatch
                    {
                        Id = fishCatchId,
                        Species = "Bass",
                        LocationName = "Lake View",
                        DateCaught = DateTime.UtcNow,
                        Photos = new List<Photo> { new Photo { Url = "/images/photo1.jpg" } },
                        User = new ApplicationUser { UserName = "TestUser" }
                    }
                }
            };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(favorites);
            _favoriteRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetUserFavoritesAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            var favorite = result.First();
            Assert.AreEqual(fishCatchId, favorite.FishCatchId);
            Assert.AreEqual("Bass", favorite.Species);
            Assert.AreEqual("Lake View", favorite.LocationName);
            Assert.AreEqual("TestUser", favorite.PublisherName);
            Assert.AreEqual(1, favorite.PhotoURLs.Count);
        }

        [Test]
        public async Task GetUserFavoritesAsync_Should_Return_Empty_If_No_Favorites()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockDbSet = DbSetMockHelper.CreateMockDbSet(new List<Favorite>());
            _favoriteRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetUserFavoritesAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task RemoveFavoriteAsync_Should_Remove_Favorite_If_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();
            var favorite = new Favorite { UserId = userId, FishCatchId = fishCatchId };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(new List<Favorite> { favorite });
            _favoriteRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            _favoriteRepositoryMock.Setup(repo => repo.DeleteByCompositeKeyAsync(userId, fishCatchId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.RemoveFavoriteAsync(userId, fishCatchId);

            // Assert
            Assert.IsTrue(result);
            _favoriteRepositoryMock.Verify(repo => repo.DeleteByCompositeKeyAsync(userId, fishCatchId), Times.Once);
        }

        [Test]
        public async Task RemoveFavoriteAsync_Should_Return_False_If_Favorite_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();
            var mockDbSet = DbSetMockHelper.CreateMockDbSet(new List<Favorite>());

            _favoriteRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _service.RemoveFavoriteAsync(userId, fishCatchId);

            // Assert
            Assert.IsFalse(result);
            _favoriteRepositoryMock.Verify(repo => repo.DeleteByCompositeKeyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}
