using FinFinder.Data.Models;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Controllers;
using FinFinder.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Tests.Controller.Tests
{
    [TestFixture]
    public class CommentControllerTests
    {
        private Mock<ICommentService> _commentServiceMock;
        private CommentController _controller;

        [SetUp]
        public void SetUp()
        {
            _commentServiceMock = new Mock<ICommentService>();

            _controller = new CommentController(_commentServiceMock.Object);

            // Mock user claims for authentication
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task Add_Should_Redirect_To_Details_If_ModelState_Is_Invalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Content", "Required");
            var model = new AddCommentViewModel { FishCatchId = Guid.NewGuid(), Content = "" };

            // Act
            var result = await _controller.Add(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("FishCatch", redirectResult.ControllerName);
            Assert.AreEqual(model.FishCatchId, redirectResult.RouteValues["id"]);
        }

        [Test]
        public async Task Add_Should_Call_Service_And_Redirect_To_Details_On_Success()
        {
            // Arrange
            var model = new AddCommentViewModel { FishCatchId = Guid.NewGuid(), Content = "Nice catch!" };

            // Act
            var result = await _controller.Add(model);

            // Assert
            _commentServiceMock.Verify(service => service.AddCommentAsync(It.IsAny<Guid>(), model), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("FishCatch", redirectResult.ControllerName);
            Assert.AreEqual(model.FishCatchId, redirectResult.RouteValues["id"]);
        }

        [Test]
        public async Task Delete_Should_Return_Unauthorized_If_Comment_Does_Not_Belong_To_User()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();
            var comment = new Comment { Id = commentId, FishCatchId = fishCatchId, UserId = Guid.NewGuid() };

            _commentServiceMock.Setup(service => service.GetCommentByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Delete_Should_Return_BadRequest_If_Deletion_Fails()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var comment = new Comment { Id = commentId, FishCatchId = fishCatchId, UserId = userId };

            _commentServiceMock.Setup(service => service.GetCommentByIdAsync(commentId))
                .ReturnsAsync(comment);

            _commentServiceMock.Setup(service => service.DeleteCommentAsync(commentId, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Unable to delete the comment.", badRequestResult.Value);
        }

        [Test]
        public async Task Delete_Should_Call_Service_And_Redirect_To_Details_On_Success()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var comment = new Comment { Id = commentId, FishCatchId = fishCatchId, UserId = userId };

            _commentServiceMock.Setup(service => service.GetCommentByIdAsync(commentId))
                .ReturnsAsync(comment);

            _commentServiceMock.Setup(service => service.DeleteCommentAsync(commentId, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            _commentServiceMock.Verify(service => service.DeleteCommentAsync(commentId, userId), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("FishCatch", redirectResult.ControllerName);
            Assert.AreEqual(comment.FishCatchId, redirectResult.RouteValues["id"]);
        }
    }

}
