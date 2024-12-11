using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Controllers;
using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
    public class LikeControllerTests
    {
        private Mock<ILikeService> _likeServiceMock;
        private LikeController _controller;

        [SetUp]
        public void SetUp()
        {
            _likeServiceMock = new Mock<ILikeService>();

            _controller = new LikeController(_likeServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                    }))
                    }
                }
            };
        }

        [Test]
        public async Task Add_Should_Add_Like_And_Redirect_To_Details_When_Success()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            _likeServiceMock.Setup(s => s.AddLikeAsync(fishCatchId, It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Add(fishCatchId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("FishCatch", redirectResult.ControllerName);
            Assert.AreEqual(fishCatchId, redirectResult.RouteValues["id"]);
        }

        [Test]
        public async Task Add_Should_Set_TempData_Error_If_Like_Already_Exists()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            _likeServiceMock.Setup(service => service.AddLikeAsync(fishCatchId, userId))
                .ReturnsAsync(false); // Simulate the like already exists

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }))
                }
            };

            // Act
            var result = await _controller.Add(fishCatchId);

            // Assert
            Assert.AreEqual("You have already liked this post.", _controller.TempData["Error"]);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("FishCatch", redirectResult.ControllerName);
            Assert.AreEqual(fishCatchId, redirectResult.RouteValues["id"]);
        }


        [Test]
        public async Task Remove_Should_Remove_Like_And_Redirect_To_Details_When_Success()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            _likeServiceMock.Setup(s => s.RemoveLikeAsync(fishCatchId, It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Remove(fishCatchId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("FishCatch", redirectResult.ControllerName);
            Assert.AreEqual(fishCatchId, redirectResult.RouteValues["id"]);
        }

        [Test]
        public async Task Remove_Should_Set_TempData_Error_If_Like_Does_Not_Exist()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            _likeServiceMock.Setup(service => service.RemoveLikeAsync(fishCatchId, userId))
                .ReturnsAsync(false); // Simulate the like does not exist

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }))
                }
            };

            // Act
            var result = await _controller.Remove(fishCatchId);

            // Assert
            Assert.AreEqual("You have not liked this post.", _controller.TempData["Error"]);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("FishCatch", redirectResult.ControllerName);
            Assert.AreEqual(fishCatchId, redirectResult.RouteValues["id"]);
        }


        [Test]
        public async Task LikedPosts_Should_Return_View_With_Liked_Posts()
        {
            // Arrange
            var likedPosts = new List<FishCatchIndexViewModel>
        {
            new FishCatchIndexViewModel { Id = Guid.NewGuid(), Species = "Bass" },
            new FishCatchIndexViewModel { Id = Guid.NewGuid(), Species = "Salmon" }
        };

            _likeServiceMock.Setup(s => s.GetLikedPostsAsync(It.IsAny<Guid>())).ReturnsAsync(likedPosts);

            // Act
            var result = await _controller.LikedPosts();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(likedPosts, viewResult.Model);
        }
    }

}
