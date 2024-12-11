using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data;
using FinFinder.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Tests.Service.Tests
{
    [TestFixture]
    public class HomeServiceTests
    {
        private Mock<IRepository<FishCatch, Guid>> _fishCatchRepositoryMock;
        private Mock<IRepository<FishingTechnique, Guid>> _fishingTechniqueRepositoryMock;
        private Mock<IRepository<Comment, Guid>> _commentRepositoryMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private HomeService _service;

        [SetUp]
        public void SetUp()
        {
            _fishCatchRepositoryMock = new Mock<IRepository<FishCatch, Guid>>();
            _fishingTechniqueRepositoryMock = new Mock<IRepository<FishingTechnique, Guid>>();
            _commentRepositoryMock = new Mock<IRepository<Comment, Guid>>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            _service = new HomeService(
                _fishCatchRepositoryMock.Object,
                _fishingTechniqueRepositoryMock.Object,
                _userManagerMock.Object,
                _commentRepositoryMock.Object);
        }

        [Test]
        public async Task GetFeaturedFishCatchesAsync_Should_Return_Top_5_Most_Liked()
        {
            // Arrange
            var fishCatches = new List<FishCatch>
    {
        new FishCatch
        {
            Id = Guid.NewGuid(),
            Species = "Bass",
            Likes = new List<Like> { new Like() },
            IsDeleted = false,
            User = new ApplicationUser { UserName = "User1" },
            Photos = new List<Photo> { new Photo { Url = "/images/photo1.jpg" } },
            DateCaught = DateTime.UtcNow.AddDays(-1)
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            Species = "Trout",
            Likes = new List<Like> { new Like(), new Like() },
            IsDeleted = false,
            User = new ApplicationUser { UserName = "User2" },
            Photos = new List<Photo> { new Photo { Url = "/images/photo2.jpg" } },
            DateCaught = DateTime.UtcNow.AddDays(-2)
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            Species = "Salmon",
            Likes = new List<Like>(),
            IsDeleted = false,
            User = new ApplicationUser { UserName = "User3" },
            Photos = new List<Photo>(),
            DateCaught = DateTime.UtcNow.AddDays(-3)
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            Species = "Catfish",
            Likes = new List<Like>(),
            IsDeleted = true, // Soft-deleted
            User = new ApplicationUser { UserName = "User4" },
            Photos = new List<Photo>(),
            DateCaught = DateTime.UtcNow.AddDays(-4)
        }
    }.AsQueryable();

            var fishCatchesMock = new Mock<DbSet<FishCatch>>();
            fishCatchesMock.As<IAsyncEnumerable<FishCatch>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<FishCatch>(fishCatches.GetEnumerator()));

            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<FishCatch>(fishCatches.Provider));
            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.Expression).Returns(fishCatches.Expression);
            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.ElementType).Returns(fishCatches.ElementType);
            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.GetEnumerator()).Returns(fishCatches.GetEnumerator());

            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(fishCatchesMock.Object);

            // Act
            var result = await _service.GetFeaturedFishCatchesAsync();

            // Assert
            Assert.AreEqual(3, result.Count); // Excludes the soft-deleted item
            Assert.AreEqual("Trout", result.First().Species); // Most liked fish catch
            Assert.AreEqual("/images/photo2.jpg", result.First().PhotoURLs.First());
        }



        [Test]
        public async Task GetMostPopularTechniqueAsync_Should_Return_Technique_With_Most_Catches()
        {
            // Arrange
            var fishingTechniques = new List<FishingTechnique>
    {
        new FishingTechnique
        {
            Id = Guid.NewGuid(),
            Name = "Technique A",
            FishCatches = new List<FishCatch> { new FishCatch(), new FishCatch() } // 2 catches
        },
        new FishingTechnique
        {
            Id = Guid.NewGuid(),
            Name = "Technique B",
            FishCatches = new List<FishCatch> { new FishCatch() } // 1 catch
        },
        new FishingTechnique
        {
            Id = Guid.NewGuid(),
            Name = "Technique C",
            FishCatches = new List<FishCatch>() // 0 catches
        }
    }.AsQueryable();

            var fishingTechniquesMock = new Mock<DbSet<FishingTechnique>>();
            fishingTechniquesMock.As<IAsyncEnumerable<FishingTechnique>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<FishingTechnique>(fishingTechniques.GetEnumerator()));

            fishingTechniquesMock.As<IQueryable<FishingTechnique>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<FishingTechnique>(fishingTechniques.Provider));
            fishingTechniquesMock.As<IQueryable<FishingTechnique>>().Setup(m => m.Expression)
                .Returns(fishingTechniques.Expression);
            fishingTechniquesMock.As<IQueryable<FishingTechnique>>().Setup(m => m.ElementType)
                .Returns(fishingTechniques.ElementType);
            fishingTechniquesMock.As<IQueryable<FishingTechnique>>().Setup(m => m.GetEnumerator())
                .Returns(fishingTechniques.GetEnumerator());

            _fishingTechniqueRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(fishingTechniquesMock.Object);

            // Act
            var result = await _service.GetMostPopularTechniqueAsync();

            // Assert
            Assert.AreEqual("Technique A", result); // Technique A has the most catches
        }


        [Test]
        public async Task GetRecentActivitiesAsync_Should_Return_Last_5_Catches()
        {
            // Arrange
            var fishCatches = new List<FishCatch>
    {
        new FishCatch
        {
            Id = Guid.NewGuid(),
            User = new ApplicationUser { UserName = "User1" },
            Species = "Bass",
            DateCaught = DateTime.UtcNow.AddDays(-1)
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            User = new ApplicationUser { UserName = "User2" },
            Species = "Salmon",
            DateCaught = DateTime.UtcNow.AddDays(-2)
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            User = new ApplicationUser { UserName = "User3" },
            Species = "Trout",
            DateCaught = DateTime.UtcNow.AddDays(-3)
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            User = new ApplicationUser { UserName = "User4" },
            Species = "Carp",
            DateCaught = DateTime.UtcNow.AddDays(-4)
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            User = new ApplicationUser { UserName = "User5" },
            Species = "Pike",
            DateCaught = DateTime.UtcNow.AddDays(-5)
        }
    }.AsQueryable();

            var fishCatchesMock = new Mock<DbSet<FishCatch>>();
            fishCatchesMock.As<IAsyncEnumerable<FishCatch>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<FishCatch>(fishCatches.GetEnumerator()));

            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<FishCatch>(fishCatches.Provider));
            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.Expression)
                .Returns(fishCatches.Expression);
            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.ElementType)
                .Returns(fishCatches.ElementType);
            fishCatchesMock.As<IQueryable<FishCatch>>().Setup(m => m.GetEnumerator())
                .Returns(fishCatches.GetEnumerator());

            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(fishCatchesMock.Object);

            // Act
            var result = await _service.GetRecentActivitiesAsync();

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("User1", result.First().UserName);
            Assert.AreEqual("posted a new catch: Bass", result.First().ActionDescription);
            Assert.AreEqual("User5", result.Last().UserName);
            Assert.AreEqual("posted a new catch: Pike", result.Last().ActionDescription);
        }

        [Test]
        public async Task GetTotalCommentsAsync_Should_Return_Total_Comments_Count()
        {
            // Arrange
            var comments = new List<Comment>
    {
        new Comment { Id = Guid.NewGuid(), Content = "Comment 1" },
        new Comment { Id = Guid.NewGuid(), Content = "Comment 2" },
        new Comment { Id = Guid.NewGuid(), Content = "Comment 3" }
    }.AsQueryable();

            var commentsMock = new Mock<DbSet<Comment>>();
            commentsMock.As<IAsyncEnumerable<Comment>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Comment>(comments.GetEnumerator()));

            commentsMock.As<IQueryable<Comment>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Comment>(comments.Provider));
            commentsMock.As<IQueryable<Comment>>().Setup(m => m.Expression)
                .Returns(comments.Expression);
            commentsMock.As<IQueryable<Comment>>().Setup(m => m.ElementType)
                .Returns(comments.ElementType);
            commentsMock.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator())
                .Returns(comments.GetEnumerator());

            _commentRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(commentsMock.Object);

            // Act
            var result = await _service.GetTotalCommentsAsync();

            // Assert
            Assert.AreEqual(3, result);
        }


        [Test]
        public async Task GetTotalFishCatchesAsync_Should_Return_Total_Catches_Count()
        {
            // Arrange
            var fishCatches = new List<FishCatch>
    {
        new FishCatch { Id = Guid.NewGuid(), Species = "Bass", IsDeleted = false },
        new FishCatch { Id = Guid.NewGuid(), Species = "Salmon", IsDeleted = false },
        new FishCatch { Id = Guid.NewGuid(), Species = "Trout", IsDeleted = true }
    }.AsQueryable();

            var fishCatchMock = new Mock<DbSet<FishCatch>>();
            fishCatchMock.As<IAsyncEnumerable<FishCatch>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<FishCatch>(fishCatches.GetEnumerator()));

            fishCatchMock.As<IQueryable<FishCatch>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<FishCatch>(fishCatches.Provider));

            fishCatchMock.As<IQueryable<FishCatch>>().Setup(m => m.Expression).Returns(fishCatches.Expression);
            fishCatchMock.As<IQueryable<FishCatch>>().Setup(m => m.ElementType).Returns(fishCatches.ElementType);
            fishCatchMock.As<IQueryable<FishCatch>>().Setup(m => m.GetEnumerator()).Returns(fishCatches.GetEnumerator());

            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(fishCatchMock.Object);

            // Act
            var result = await _service.GetTotalFishCatchesAsync();

            // Assert
            Assert.AreEqual(2, result); // Only non-deleted catches are counted
        }


        [Test]
        public async Task GetTotalUsersAsync_Should_Return_Total_Users_Count()
        {
            // Arrange
            var users = new List<ApplicationUser>
    {
        new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1" },
        new ApplicationUser { Id = Guid.NewGuid(), UserName = "User2" },
        new ApplicationUser { Id = Guid.NewGuid(), UserName = "User3" }
    }.AsQueryable();

            var userMockSet = new Mock<DbSet<ApplicationUser>>();
            userMockSet.As<IAsyncEnumerable<ApplicationUser>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ApplicationUser>(users.GetEnumerator()));

            userMockSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<ApplicationUser>(users.Provider));

            userMockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            userMockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            userMockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _userManagerMock.Setup(m => m.Users).Returns(userMockSet.Object);

            // Act
            var result = await _service.GetTotalUsersAsync();

            // Assert
            Assert.AreEqual(3, result); // Total users in the list
        }


        [Test]
        public async Task GetUserRecentCatchesAsync_Should_Return_Recent_Catches_For_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fishCatches = new List<FishCatch>
    {
        new FishCatch
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Species = "Bass",
            LocationName = "Lake A",
            DateCaught = DateTime.UtcNow.AddDays(-1),
            Photos = new List<Photo> { new Photo { Url = "/images/photo1.jpg" } }
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Species = "Trout",
            LocationName = "River B",
            DateCaught = DateTime.UtcNow.AddDays(-2),
            Photos = new List<Photo> { new Photo { Url = "/images/photo2.jpg" } }
        },
        new FishCatch
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Species = "Salmon",
            LocationName = "Ocean C",
            DateCaught = DateTime.UtcNow.AddDays(-3),
            Photos = new List<Photo> { new Photo { Url = "/images/photo3.jpg" } }
        }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<FishCatch>>();
            mockDbSet.As<IAsyncEnumerable<FishCatch>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<FishCatch>(fishCatches.GetEnumerator()));

            mockDbSet.As<IQueryable<FishCatch>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<FishCatch>(fishCatches.Provider));

            mockDbSet.As<IQueryable<FishCatch>>().Setup(m => m.Expression).Returns(fishCatches.Expression);
            mockDbSet.As<IQueryable<FishCatch>>().Setup(m => m.ElementType).Returns(fishCatches.ElementType);
            mockDbSet.As<IQueryable<FishCatch>>().Setup(m => m.GetEnumerator()).Returns(fishCatches.GetEnumerator());

            _fishCatchRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetUserRecentCatchesAsync(userId);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Bass", result[0].Species);
            Assert.AreEqual("Trout", result[1].Species);
            Assert.AreEqual("Salmon", result[2].Species);
            Assert.AreEqual("/images/photo1.jpg", result[0].PhotoURLs.First());
        }

    }

}
