using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using Moq.EntityFrameworkCore;
using FinFinder.Web.Infrastructure.Extensions;

namespace FinFinder.Tests.Service.Tests
{
    [TestFixture]
    public class FishCatchServiceTests
    {
        private Mock<IRepository<FishCatch, Guid>> _fishCatchRepositoryMock;
        private Mock<IRepository<FishingTechnique, Guid>> _fishingTechniqueRepositoryMock;
        private Mock<IRepository<Photo, Guid>> _photoRepositoryMock;
        private Mock<ICompositeKeyRepository<Favorite, Guid, Guid>> _favoriteRepositoryMock;
        private Mock<IRepository<Comment, Guid>> _commentRepositoryMock;
        private Mock<IRepository<Like, Guid>> _likeRepositoryMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private FishCatchService _service;
        [SetUp]
        public void Setup()
        {
            _fishCatchRepositoryMock = new Mock<IRepository<FishCatch, Guid>>();
            _fishingTechniqueRepositoryMock = new Mock<IRepository<FishingTechnique, Guid>>();
            _photoRepositoryMock = new Mock<IRepository<Photo, Guid>>();
            _favoriteRepositoryMock = new Mock<ICompositeKeyRepository<Favorite, Guid, Guid>>();
            _commentRepositoryMock = new Mock<IRepository<Comment, Guid>>();
            _likeRepositoryMock = new Mock<IRepository<Like, Guid>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _service = new FishCatchService(
                _fishCatchRepositoryMock.Object,
                _fishingTechniqueRepositoryMock.Object,
                _photoRepositoryMock.Object,
                _favoriteRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _likeRepositoryMock.Object,
                _userManagerMock.Object
            );
        }

        [Test]
        public async Task AddToFavoritesAsync_Should_Return_False_If_Already_Exists()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingFavorites = new List<Favorite>
    {
        new Favorite { FishCatchId = fishCatchId, UserId = userId }
    }.AsQueryable();

            // Set up async LINQ for the mock
            var mockSet = new Mock<DbSet<Favorite>>();
            mockSet.As<IAsyncEnumerable<Favorite>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Favorite>(existingFavorites.GetEnumerator()));

            mockSet.As<IQueryable<Favorite>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Favorite>(existingFavorites.Provider));
            mockSet.As<IQueryable<Favorite>>().Setup(m => m.Expression).Returns(existingFavorites.Expression);
            mockSet.As<IQueryable<Favorite>>().Setup(m => m.ElementType).Returns(existingFavorites.ElementType);
            mockSet.As<IQueryable<Favorite>>().Setup(m => m.GetEnumerator()).Returns(existingFavorites.GetEnumerator());

            _favoriteRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockSet.Object);

            // Act
            var result = await _service.AddToFavoritesAsync(fishCatchId, userId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateFishCatchAsync_Should_Return_False_If_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new FishCatchCreateViewModel();

            _userManagerMock
                .Setup(um => um.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _service.CreateFishCatchAsync(model, userId);

            // Assert
            Assert.IsFalse(result);
            _fishCatchRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<FishCatch>()), Times.Never);
        }

        [Test]
        public async Task CreateFishCatchAsync_Should_Add_FishCatch_Without_Photos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, FishCount = 0 };
            var model = new FishCatchCreateViewModel { Species = "Trout" };

            _userManagerMock
                .Setup(um => um.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _fishCatchRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<FishCatch>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateFishCatchAsync(model, userId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, user.FishCount);
            _fishCatchRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<FishCatch>()), Times.Once);
        }

        [Test]
        public async Task GetFilteredFishCatchesAsync_Should_Filter_By_MostLiked()
        {
            // Arrange
            var fishCatches = new List<FishCatch>
    {
        new FishCatch
        {
            Id = Guid.NewGuid(),
            Species = "Fish A",
            Likes = new List<Like> { new Like { Id = Guid.NewGuid() }, new Like { Id = Guid.NewGuid() } }
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            Species = "Fish B",
            Likes = new List<Like> { new Like { Id = Guid.NewGuid() } }
        }
    }.AsQueryable();

            // Mock GetAllAttached to support async
            var mockSet = new Mock<DbSet<FishCatch>>();
            mockSet.As<IAsyncEnumerable<FishCatch>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<FishCatch>(fishCatches.GetEnumerator()));

            mockSet.As<IQueryable<FishCatch>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<FishCatch>(fishCatches.Provider));
            mockSet.As<IQueryable<FishCatch>>().Setup(m => m.Expression).Returns(fishCatches.Expression);
            mockSet.As<IQueryable<FishCatch>>().Setup(m => m.ElementType).Returns(fishCatches.ElementType);
            mockSet.As<IQueryable<FishCatch>>().Setup(m => m.GetEnumerator()).Returns(fishCatches.GetEnumerator());

            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockSet.Object);

            // Act
            var result = await _service.GetFilteredFishCatchesAsync("MostLiked", "");

            // Assert
            Assert.AreEqual(2, result.FishCatches.Count);
            Assert.AreEqual("Fish A", result.FishCatches.First().Species); // Most liked
        }
        [Test]
        public async Task PermanentDeleteFishCatchAsync_Should_Return_False_If_User_Not_Owner()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var fishCatch = new FishCatch { Id = fishCatchId, UserId = Guid.NewGuid() };

            _fishCatchRepositoryMock
                .Setup(repo => repo.GetAllAttached())
                .Returns(new List<FishCatch> { fishCatch }.AsQueryable());

            // Act
            var result = await _service.PermanentDeleteFishCatchAsync(fishCatchId, userId);

            // Assert
            Assert.IsFalse(result);
            _fishCatchRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }




        [Test]
        public async Task GetFishCatchByIdAsync_Should_Return_FishCatch_If_Exists()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var fishCatch = new FishCatch { Id = fishCatchId, Species = "Trout" };

            _fishCatchRepositoryMock
                .Setup(repo => repo.GetByIdAsync(fishCatchId))
                .ReturnsAsync(fishCatch);

            // Act
            var result = await _service.GetFishCatchByIdAsync(fishCatchId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fishCatchId, result.Id);
            Assert.AreEqual("Trout", result.Species);
        }

        [Test]
        public async Task GetFishCatchByIdAsync_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();

            _fishCatchRepositoryMock
                .Setup(repo => repo.GetByIdAsync(fishCatchId))
                .ReturnsAsync((FishCatch)null);

            // Act
            var result = await _service.GetFishCatchByIdAsync(fishCatchId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task SoftDeleteFishCatchAsync_Should_Soft_Delete_If_User_Is_Owner()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var fishCatch = new FishCatch
            {
                Id = fishCatchId,
                UserId = userId,
                Species = "Salmon",
                Description = "A fresh catch",
                IsDeleted = false
            };

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "TestUser",
                FishCount = 10
            };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(new List<FishCatch> { fishCatch });
            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            _userManagerMock.Setup(manager => manager.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _fishCatchRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<FishCatch>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.SoftDeleteFishCatchAsync(fishCatchId, userId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(fishCatch.IsDeleted, "The fish catch should be marked as deleted.");
            Assert.AreEqual(9, user.FishCount, "User's fish count should decrement by 1.");
        }


        [Test]
        public async Task SoftDeleteFishCatchAsync_Should_Return_False_If_Not_Owner()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var fishCatch = new FishCatch { Id = fishCatchId, UserId = Guid.NewGuid(), IsDeleted = false };

            _fishCatchRepositoryMock
                .Setup(repo => repo.GetAllAttached())
                .Returns(new List<FishCatch> { fishCatch }.AsQueryable());

            // Act
            var result = await _service.SoftDeleteFishCatchAsync(fishCatchId, userId);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(fishCatch.IsDeleted);
        }

        [Test]
        public async Task UpdateFishCatchAsync_Should_Update_If_User_Is_Owner()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var fishCatch = new FishCatch
            {
                Id = fishCatchId,
                UserId = userId,
                Species = "Bass",
                Description = "A big fish",
                Weight = 5.0,
                Length = 30.0,
                Latitude = 123.45,
                Longitude = 678.90,
                LocationName = "Lake View",
                FishingTechniqueId = Guid.NewGuid(),
                IsDeleted = false,
                Photos = new List<Photo> { new Photo { Id = Guid.NewGuid(), Url = "/images/photo1.jpg" } }
            };

            var updatedFishCatch = new FishCatchEditViewModel
            {
                Id = fishCatchId,
                Species = "Salmon",
                Description = "An updated description",
                Weight = 7.5,
                Length = 40.0,
                Latitude = 223.45,
                Longitude = 778.90,
                LocationName = "Ocean View",
                FishingTechniqueId = Guid.NewGuid(),
                PhotosToRemove = new List<Guid> { fishCatch.Photos.First().Id },
                NewPhotoFiles = null
            };

            var user = new ApplicationUser { Id = userId, UserName = "TestUser" };

            var fishCatches = new List<FishCatch> { fishCatch };

            var mockFishCatches = DbSetMockHelper.CreateMockDbSet(fishCatches);
            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(mockFishCatches.Object);

            var mockUsers = DbSetMockHelper.CreateMockDbSet(new List<ApplicationUser> { user });
            _userManagerMock.Setup(manager => manager.Users)
                .Returns(mockUsers.Object);


            _userManagerMock.Setup(manager => manager.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Administrator"))
                .ReturnsAsync(false);

            _fishCatchRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<FishCatch>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateFishCatchAsync(updatedFishCatch, userId);

            // Assert
            Assert.IsTrue(result);
            _fishCatchRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<FishCatch>(fc =>
                fc.Id == fishCatchId &&
                fc.Species == updatedFishCatch.Species &&
                fc.Description == updatedFishCatch.Description &&
                fc.Weight == updatedFishCatch.Weight &&
                fc.Length == updatedFishCatch.Length &&
                fc.Latitude == updatedFishCatch.Latitude &&
                fc.Longitude == updatedFishCatch.Longitude &&
                fc.LocationName == updatedFishCatch.LocationName &&
                fc.FishingTechniqueId == updatedFishCatch.FishingTechniqueId
            )), Times.Once);

            _photoRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Once);
        }


        [Test]
        public async Task UpdateFishCatchAsync_Should_Return_False_If_Not_Owner()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid(); // Current user
            var ownerId = Guid.NewGuid(); // FishCatch owner

            var fishCatch = new FishCatch
            {
                Id = fishCatchId,
                UserId = ownerId, // Owner is different from current user
                Species = "Salmon",
                Description = "A fresh catch",
                IsDeleted = false
            };

            var model = new FishCatchEditViewModel
            {
                Id = fishCatchId,
                Species = "Updated Salmon",
                Description = "Updated description",
                Weight = 10,
                Length = 100
            };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(new List<FishCatch> { fishCatch });
            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            _userManagerMock.Setup(manager => manager.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new ApplicationUser { Id = userId, UserName = "TestUser" });

            _userManagerMock.Setup(manager => manager.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Administrator"))
                .ReturnsAsync(false); // Current user is not an admin

            // Act
            var result = await _service.UpdateFishCatchAsync(model, userId);

            // Assert
            Assert.IsFalse(result, "The method should return false if the user is not the owner or an admin.");
        }







    }
}