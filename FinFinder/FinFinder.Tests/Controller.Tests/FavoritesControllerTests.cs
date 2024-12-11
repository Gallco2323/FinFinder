using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Controllers;
using FinFinder.Web.ViewModels.FishCatch;
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
    public class FavoritesControllerTests
    {
        private Mock<IFavoriteService> _favoriteServiceMock;
        private FavoritesController _controller;

        [SetUp]
        public void SetUp()
        {
            _favoriteServiceMock = new Mock<IFavoriteService>();
            _controller = new FavoritesController(null, _favoriteServiceMock.Object);

            // Mocking user context
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task Index_Should_Return_View_With_Favorites()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = new List<FishCatchFavoriteViewModel>
        {
            new FishCatchFavoriteViewModel { FishCatchId = Guid.NewGuid(), Species = "Bass", LocationName = "Lake View", PublisherName = "John Doe" },
            new FishCatchFavoriteViewModel { FishCatchId = Guid.NewGuid(), Species = "Trout", LocationName = "River Bend", PublisherName = "Jane Doe" }
        };

            _favoriteServiceMock.Setup(service => service.GetUserFavoritesAsync(userId))
                .ReturnsAsync(favorites);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(favorites, viewResult.Model);
        }

        [Test]
        public async Task Remove_Should_Call_Service_And_Redirect_To_Index()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var fishCatchId = Guid.NewGuid();

            _favoriteServiceMock.Setup(service => service.RemoveFavoriteAsync(userId, fishCatchId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Remove(fishCatchId);

            // Assert
            _favoriteServiceMock.Verify(service => service.RemoveFavoriteAsync(userId, fishCatchId), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [Test]
        public async Task Remove_Should_Handle_Service_Failure_And_Redirect_To_Index()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var fishCatchId = Guid.NewGuid();

            _favoriteServiceMock.Setup(service => service.RemoveFavoriteAsync(userId, fishCatchId))
                .ReturnsAsync(false); // Simulate service failure

            // Act
            var result = await _controller.Remove(fishCatchId);

            // Assert
            _favoriteServiceMock.Verify(service => service.RemoveFavoriteAsync(userId, fishCatchId), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }
    }

}
