using FinFinder.Controllers;
using FinFinder.Models;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class HomeControllerTests
    {
        private Mock<IHomeService> _homeServiceMock;
        private Mock<ILogger<HomeController>> _loggerMock;
        private HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _homeServiceMock = new Mock<IHomeService>();
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_homeServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Index_Should_Return_View_With_HomePageViewModel()
        {
            // Arrange
            var featuredCatches = new List<FishCatchIndexViewModel>
    {
        new FishCatchIndexViewModel { Id = Guid.NewGuid(), Species = "Bass" }
    };

            var recentActivities = new List<ActivityViewModel>
    {
        new ActivityViewModel { UserName = "John", ActionDescription = "posted a catch" }
    };

            _homeServiceMock.Setup(s => s.GetFeaturedFishCatchesAsync()).ReturnsAsync(featuredCatches);
            _homeServiceMock.Setup(s => s.GetTotalFishCatchesAsync()).ReturnsAsync(10);
            _homeServiceMock.Setup(s => s.GetTotalUsersAsync()).ReturnsAsync(5);
            _homeServiceMock.Setup(s => s.GetMostPopularTechniqueAsync()).ReturnsAsync("Fly Fishing");
            _homeServiceMock.Setup(s => s.GetRecentActivitiesAsync()).ReturnsAsync(recentActivities);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<HomePageViewModel>(viewResult.Model);

            var model = (HomePageViewModel)viewResult.Model;

            // Validate model properties
            Assert.AreEqual(10, model.TotalFishCatches);
            Assert.AreEqual(5, model.TotalUsers);
            Assert.AreEqual("Fly Fishing", model.MostPopularTechnique);
            Assert.AreEqual(1, model.FeaturedFishCatches.Count); // Ensure FeaturedFishCatches has 1 item
            Assert.AreEqual("Bass", model.FeaturedFishCatches.First().Species);
        }


        [Test]
        public async Task Index_Should_Return_View_With_Empty_UserRecentCatches_If_Not_Authenticated()
        {
            // Arrange
            var featuredCatches = new List<FishCatchIndexViewModel> { new FishCatchIndexViewModel { Id = Guid.NewGuid(), Species = "Bass" } };
            var recentActivities = new List<ActivityViewModel> { new ActivityViewModel { UserName = "John", ActionDescription = "posted a catch" } };

            _homeServiceMock.Setup(s => s.GetFeaturedFishCatchesAsync()).ReturnsAsync(featuredCatches);
            _homeServiceMock.Setup(s => s.GetTotalFishCatchesAsync()).ReturnsAsync(10);
            _homeServiceMock.Setup(s => s.GetTotalUsersAsync()).ReturnsAsync(5);
            _homeServiceMock.Setup(s => s.GetMostPopularTechniqueAsync()).ReturnsAsync("Fly Fishing");
            _homeServiceMock.Setup(s => s.GetRecentActivitiesAsync()).ReturnsAsync(recentActivities);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()) // No authentication claims
                }
            };

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<HomePageViewModel>(viewResult.Model);

            var model = (HomePageViewModel)viewResult.Model;
            Assert.AreEqual(10, model.TotalFishCatches);
            Assert.AreEqual(5, model.TotalUsers);
            Assert.AreEqual("Fly Fishing", model.MostPopularTechnique);
            Assert.AreEqual(1, model.FeaturedFishCatches.Count);
            Assert.AreEqual(0, model.UserRecentCatches.Count); // No recent catches as the user is not authenticated
        }


        [Test]
        public void Privacy_Should_Return_View()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Error_Should_Return_View_With_ErrorViewModel()
        {
            // Arrange
            var activityId = "TestActivityId";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    TraceIdentifier = activityId
                }
            };

            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<ErrorViewModel>(viewResult.Model);

            var model = (ErrorViewModel)viewResult.Model;
            Assert.AreEqual(activityId, model.RequestId);
        }
    }

}
