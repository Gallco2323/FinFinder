using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Infrastructure.Extensions;
using FinFinder.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Tests.Service.Tests
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IRepository<FishCatch, Guid>> _fishCatchRepositoryMock;
        private ProfileService _service;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _fishCatchRepositoryMock = new Mock<IRepository<FishCatch, Guid>>();
            _service = new ProfileService(_userManagerMock.Object, _fishCatchRepositoryMock.Object);
        }

        
        [Test]
        public async Task GetHiddenPostsAsync_Should_Return_Hidden_Posts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var hiddenPosts = new List<FishCatch>
    {
        new FishCatch
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsDeleted = true,
            Species = "Bass",
            LocationName = "Hidden Lake",
            DateCaught = DateTime.UtcNow,
            Photos = new List<Photo>
            {
                new Photo { Id = Guid.NewGuid(), Url = "/images/photo1.jpg" }
            }
        }
    };

            var mockDbSet = DbSetMockHelper.CreateMockDbSet(hiddenPosts);
            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetHiddenPostsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Bass", result.First().Species);
        }


        [Test]
        public async Task GetProfileDetailsAsync_Should_Return_Profile_Details()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "TestUser",
                Bio = "This is a bio",
                ProfilePictureURL = "/images/profile.jpg",
                FishCount = 5,
                FishCatches = new List<FishCatch>
        {
            new FishCatch
            {
                Id = fishCatchId,
                Species = "Bass",
                LocationName = "Lake View",
                DateCaught = DateTime.UtcNow,
                IsDeleted = false,
                Photos = new List<Photo>
                {
                    new Photo { Url = "/images/photo1.jpg" },
                    new Photo { Url = "/images/photo2.jpg" }
                }
            }
        }
            };

            var users = new List<ApplicationUser> { user };

            _userManagerMock.Setup(manager => manager.Users)
                .Returns(DbSetMockHelper.CreateMockDbSet(users).Object);

            // Act
            var result = await _service.GetProfileDetailsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserName, result.UserName);
            Assert.AreEqual(user.Bio, result.Bio);
            Assert.AreEqual(user.ProfilePictureURL, result.ProfilePictureURL);
            Assert.AreEqual(1, result.FishCatches.Count);
            Assert.AreEqual("Bass", result.FishCatches.First().Species);
            Assert.AreEqual("Lake View", result.FishCatches.First().LocationName);
        }


        [Test]
        public async Task GetProfileDetailsAsync_Should_Return_Null_If_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid(); // Non-existent user ID
            var users = new List<ApplicationUser>(); // Empty list to simulate no users

            _userManagerMock.Setup(manager => manager.Users)
                .Returns(DbSetMockHelper.CreateMockDbSet(users).Object);

            // Act
            var result = await _service.GetProfileDetailsAsync(userId);

            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task GetProfileForEditAsync_Should_Return_Edit_Model()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "TestUser",
                Bio = "This is a bio",
                ProfilePictureURL = "/images/profile.jpg"
            };

            var users = new List<ApplicationUser> { user };
            _userManagerMock.Setup(manager => manager.Users)
                .Returns(DbSetMockHelper.CreateMockDbSet(users).Object);

            // Act
            var result = await _service.GetProfileForEditAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserName, result.UserName);
            Assert.AreEqual(user.Bio, result.Bio);
            Assert.AreEqual(user.ProfilePictureURL, result.ProfilePictureURL);
        }


        [Test]
        public async Task GetProfileForEditAsync_Should_Return_Null_If_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid(); // Random userId that doesn't match any user in the mock
            var users = new List<ApplicationUser>(); // Empty list simulates no users found

            _userManagerMock.Setup(manager => manager.Users)
                .Returns(DbSetMockHelper.CreateMockDbSet(users).Object);

            // Act
            var result = await _service.GetProfileForEditAsync(userId);

            // Assert
            Assert.IsNull(result);
        }


        [Test]
        public async Task UnhidePostAsync_Should_Unhide_Post()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "TestUser",
                FishCount = 0
            };

            var fishCatch = new FishCatch
            {
                Id = fishCatchId,
                UserId = userId,
                IsDeleted = true
            };

            _userManagerMock.Setup(manager => manager.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            var fishCatches = new List<FishCatch> { fishCatch };

            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(DbSetMockHelper.CreateMockDbSet(fishCatches).Object);

            // Act
            var result = await _service.UnhidePostAsync(userId, fishCatchId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(fishCatch.IsDeleted);
            Assert.AreEqual(1, user.FishCount);

            _fishCatchRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<FishCatch>()), Times.Once);
        }


        [Test]
        public async Task UpdateProfileAsync_Should_Update_Profile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "OldUserName",
                Bio = "Old Bio",
                ProfilePictureURL = "/images/old-profile.png"
            };

            var users = new List<ApplicationUser> { user };
            _userManagerMock.Setup(m => m.Users)
                .Returns(DbSetMockHelper.CreateMockDbSet(users).Object);

            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ProfileEditViewModel
            {
                UserName = "NewUserName",
                Bio = "New Bio",
                ProfilePicture = null // No new profile picture
            };

            // Act
            var result = await _service.UpdateProfileAsync(userId, model);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("NewUserName", user.UserName);
            Assert.AreEqual("New Bio", user.Bio);
            _userManagerMock.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u => u.Id == userId)), Times.Once);
        }


        [Test]
        public async Task UpdateProfileAsync_Should_Return_False_If_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new ProfileEditViewModel
            {
                UserName = "NewUserName",
                Bio = "New Bio"
            };

            // Mocking UserManager.Users to return an empty list
            var users = new List<ApplicationUser>();
            _userManagerMock.Setup(m => m.Users)
                .Returns(DbSetMockHelper.CreateMockDbSet(users).Object);

            // Act
            var result = await _service.UpdateProfileAsync(userId, model);

            // Assert
            Assert.IsFalse(result); // Should return false as no user is found
        }
    }
}
