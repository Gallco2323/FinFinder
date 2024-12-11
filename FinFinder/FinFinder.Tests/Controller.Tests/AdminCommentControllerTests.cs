using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Areas.Admin.Controllers;
using FinFinder.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Tests.Controller.Tests
{
    [TestFixture]
    public class AdminCommentControllerTests
    {
        private AdminCommentController _controller;
        private Mock<ICommentService> _commentServiceMock;

        [SetUp]
        public void Setup()
        {
            _commentServiceMock = new Mock<ICommentService>();
            _controller = new AdminCommentController(_commentServiceMock.Object);
        }

        [Test]
        public async Task Index_Should_Return_View_With_All_Comments()
        {
            // Arrange
            var comments = new List<CommentViewModel>
        {
            new CommentViewModel { Id = Guid.NewGuid(), Content = "Comment 1", UserName = "User1" },
            new CommentViewModel { Id = Guid.NewGuid(), Content = "Comment 2", UserName = "User2" }
        };
            _commentServiceMock.Setup(service => service.GetAllCommentsAsync()).ReturnsAsync(comments);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<List<CommentViewModel>>(viewResult.Model);
            var model = viewResult.Model as List<CommentViewModel>;
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Comment 1", model[0].Content);
        }

        [Test]
        public async Task Delete_Should_RedirectToIndex_On_Success()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _commentServiceMock.Setup(service => service.AdminDeleteCommentAsync(commentId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(AdminCommentController.Index), redirectResult.ActionName);
        }

        [Test]
        public async Task Delete_Should_Set_ModelStateError_And_RedirectToIndex_On_Failure()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _commentServiceMock.Setup(service => service.AdminDeleteCommentAsync(commentId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(AdminCommentController.Index), redirectResult.ActionName);
        }
    }

}
