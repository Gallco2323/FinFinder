using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Services.Data;
using FinFinder.Web.ViewModels.Comment;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinFinder.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using FinFinder.Web.Infrastructure.Extensions;

namespace FinFinder.Tests.Service.Tests
{
    using static FinFinder.Web.Infrastructure.Extensions.DbSetMockHelper;
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<IRepository<Comment, Guid>> _commentRepositoryMock;
        private ICommentService _service;

        [SetUp]
        public void SetUp()
        {
            _commentRepositoryMock = new Mock<IRepository<Comment, Guid>>();
            _service = new CommentService(_commentRepositoryMock.Object);
        }

        [Test]
        public async Task AddCommentAsync_Should_Add_Comment()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new AddCommentViewModel
            {
                Content = "Great catch!",
                FishCatchId = Guid.NewGuid()
            };

            _commentRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            // Act
            await _service.AddCommentAsync(userId, model);

            // Assert
            _commentRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Comment>(c =>
                c.Content == model.Content &&
                c.FishCatchId == model.FishCatchId &&
                c.UserId == userId)), Times.Once);
        }

        [Test]
        public async Task CanUserDeleteCommentAsync_Should_Return_True_If_User_Is_Owner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var comment = new Comment { Id = commentId, UserId = userId };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);

            // Act
            var result = await _service.CanUserDeleteCommentAsync(commentId, userId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task CanUserDeleteCommentAsync_Should_Return_False_If_User_Is_Not_Owner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var comment = new Comment { Id = commentId, UserId = Guid.NewGuid() };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);

            // Act
            var result = await _service.CanUserDeleteCommentAsync(commentId, userId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteCommentAsync_Should_Delete_If_User_Is_Owner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var comment = new Comment { Id = commentId, UserId = userId };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(repo => repo.DeleteAsync(commentId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCommentAsync(commentId, userId);

            // Assert
            Assert.IsTrue(result);
            _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentAsync_Should_Return_False_If_User_Is_Not_Owner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var comment = new Comment { Id = commentId, UserId = Guid.NewGuid() };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);

            // Act
            var result = await _service.DeleteCommentAsync(commentId, userId);

            // Assert
            Assert.IsFalse(result);
            _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Never);
        }

        [Test]
        public async Task AdminDeleteCommentAsync_Should_Delete_If_Exists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var comment = new Comment { Id = commentId };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(repo => repo.DeleteAsync(commentId)).ReturnsAsync(true);

            // Act
            var result = await _service.AdminDeleteCommentAsync(commentId);

            // Assert
            Assert.IsTrue(result);
            _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Once);
        }

        [Test]
        public async Task AdminDeleteCommentAsync_Should_Return_False_If_Comment_Not_Found()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync((Comment)null);

            // Act
            var result = await _service.AdminDeleteCommentAsync(commentId);

            // Assert
            Assert.IsFalse(result);
        }
        [Test]

        public async Task GetAllCommentsAsync_Should_Return_All_Comments()
        {
            // Arrange
            var comments = new List<Comment>
    {
        new Comment
        {
            Id = Guid.NewGuid(),
            Content = "Comment 1",
            User = new ApplicationUser { UserName = "User1" },
            FishCatchId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow
        },
        new Comment
        {
            Id = Guid.NewGuid(),
            Content = "Comment 2",
            User = new ApplicationUser { UserName = "User2" },
            FishCatchId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow
        }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Comment>>();

            mockDbSet.As<IAsyncEnumerable<Comment>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Comment>(comments.GetEnumerator()));

            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Comment>(comments.Provider));
            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(comments.Expression);
            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(comments.ElementType);
            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(comments.GetEnumerator());

            _commentRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetAllCommentsAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Comment 1", result.First().Content);
            Assert.AreEqual("User1", result.First().UserName);
        }


        [Test]
        public async Task GetCommentsForFishCatchAsync_Should_Return_Comments_For_FishCatch()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var comments = new List<Comment>
            {
                new Comment { Id = Guid.NewGuid(), Content = "Comment 1", FishCatchId = fishCatchId },
                new Comment { Id = Guid.NewGuid(), Content = "Comment 2", FishCatchId = fishCatchId }
            };

            _commentRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
      .ReturnsAsync((Expression<Func<Comment, bool>> predicate) => comments.AsQueryable().Where(predicate.Compile()).ToList());

            // Act
            var result = await _service.GetCommentsForFishCatchAsync(fishCatchId);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(c => c.FishCatchId == fishCatchId));
        }
    }
}
