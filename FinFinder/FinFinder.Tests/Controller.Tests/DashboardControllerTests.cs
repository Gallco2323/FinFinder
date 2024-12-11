using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Areas.Admin.Controllers;
using FinFinder.Web.ViewModels.Admin;
using FinFinder.Web.ViewModels.Home;
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
    public class DashboardControllerTests
    {
        private DashboardController _controller;
        private Mock<IHomeService> _homeServiceMock;

        [SetUp]
        public void Setup()
        {
            _homeServiceMock = new Mock<IHomeService>();
            _controller = new DashboardController(_homeServiceMock.Object);
        }

        [Test]
        public async Task Index_Should_Return_View_With_AdminDashboardViewModel()
        {
            // Arrange
            var totalUsers = 50;
            var totalFishCatches = 120;
            var totalComments = 300;
            var recentActivities = new List<ActivityViewModel>
        {
            new ActivityViewModel
            {
                UserName = "User1",
                ActionDescription = "posted a new catch: Bass",
                Timestamp = DateTime.UtcNow.AddMinutes(-10)
            },
            new ActivityViewModel
            {
                UserName = "User2",
                ActionDescription = "commented on a post",
                Timestamp = DateTime.UtcNow.AddMinutes(-20)
            }
        };

            _homeServiceMock.Setup(service => service.GetTotalUsersAsync()).ReturnsAsync(totalUsers);
            _homeServiceMock.Setup(service => service.GetTotalFishCatchesAsync()).ReturnsAsync(totalFishCatches);
            _homeServiceMock.Setup(service => service.GetTotalCommentsAsync()).ReturnsAsync(totalComments);
            _homeServiceMock.Setup(service => service.GetRecentActivitiesAsync()).ReturnsAsync(recentActivities);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<AdminDashboardViewModel>(viewResult.Model);

            var model = viewResult.Model as AdminDashboardViewModel;
            Assert.AreEqual(totalUsers, model.TotalUsers);
            Assert.AreEqual(totalFishCatches, model.TotalFishCatches);
            Assert.AreEqual(totalComments, model.TotalComments);
            Assert.AreEqual(2, model.RecentActivities.Count);
            Assert.AreEqual("User1", model.RecentActivities[0].UserName);
            Assert.AreEqual("posted a new catch: Bass", model.RecentActivities[0].ActionDescription);
        }

        [Test]
        public async Task Index_Should_Return_View_With_Empty_RecentActivities_If_None_Available()
        {
            // Arrange
            var totalUsers = 0;
            var totalFishCatches = 0;
            var totalComments = 0;
            var recentActivities = new List<ActivityViewModel>();

            _homeServiceMock.Setup(service => service.GetTotalUsersAsync()).ReturnsAsync(totalUsers);
            _homeServiceMock.Setup(service => service.GetTotalFishCatchesAsync()).ReturnsAsync(totalFishCatches);
            _homeServiceMock.Setup(service => service.GetTotalCommentsAsync()).ReturnsAsync(totalComments);
            _homeServiceMock.Setup(service => service.GetRecentActivitiesAsync()).ReturnsAsync(recentActivities);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<AdminDashboardViewModel>(viewResult.Model);

            var model = viewResult.Model as AdminDashboardViewModel;
            Assert.AreEqual(totalUsers, model.TotalUsers);
            Assert.AreEqual(totalFishCatches, model.TotalFishCatches);
            Assert.AreEqual(totalComments, model.TotalComments);
            Assert.IsEmpty(model.RecentActivities);
        }
    }

}
