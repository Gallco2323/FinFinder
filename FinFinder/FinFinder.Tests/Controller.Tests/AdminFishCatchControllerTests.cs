using FinFinder.Data.Models;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Areas.Admin.Controllers;
using FinFinder.Web.ViewModels.FishCatch;
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
    public class AdminFishCatchControllerTests
    {
        private AdminFishCatchController _controller;
        private Mock<IFishCatchService> _fishCatchServiceMock;

        [SetUp]
        public void Setup()
        {
            _fishCatchServiceMock = new Mock<IFishCatchService>();
            _controller = new AdminFishCatchController(_fishCatchServiceMock.Object);
        }

        [Test]
        public async Task Index_Should_Return_View_With_ManageFishCatchViewModel()
        {
            // Arrange
            var fishCatches = new List<ManageFishCatchViewModel>
        {
            new ManageFishCatchViewModel
            {
                Id = Guid.NewGuid(),
                Species = "Bass",
                LocationName = "Lake View",
                PublisherName = "User1",
                DateCaught = DateTime.UtcNow,
                PhotoURLs = new List<string> { "/images/photo1.jpg" },
                IsDeleted = false
            },
            new ManageFishCatchViewModel
            {
                Id = Guid.NewGuid(),
                Species = "Salmon",
                LocationName = "River Bank",
                PublisherName = "User2",
                DateCaught = DateTime.UtcNow.AddDays(-1),
                PhotoURLs = new List<string> { "/images/photo2.jpg" },
                IsDeleted = true
            }
        };

            _fishCatchServiceMock.Setup(service => service.GetAllFishCatchesAsync())
                .ReturnsAsync(fishCatches);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<List<ManageFishCatchViewModel>>(viewResult.Model);
            var model = viewResult.Model as List<ManageFishCatchViewModel>;
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Bass", model[0].Species);
            Assert.AreEqual("Salmon", model[1].Species);
        }

        [Test]
        public async Task Delete_Should_RedirectToIndex_On_Success()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var fishCatch = new FishCatch
            {
                Id = fishCatchId,
                UserId = Guid.NewGuid()
            };

            _fishCatchServiceMock.Setup(service => service.GetFishCatchByIdAsync(fishCatchId))
                .ReturnsAsync(fishCatch);
            _fishCatchServiceMock.Setup(service => service.PermanentDeleteFishCatchAsync(fishCatchId, fishCatch.UserId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(fishCatchId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(AdminFishCatchController.Index), redirectResult.ActionName);
        }

        [Test]
        public async Task Delete_Should_Return_NotFound_If_FishCatch_Not_Found()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();

            _fishCatchServiceMock.Setup(service => service.GetFishCatchByIdAsync(fishCatchId))
                .ReturnsAsync((FishCatch)null);

            // Act
            var result = await _controller.Delete(fishCatchId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Fish catch not found.", notFoundResult.Value);
        }

        [Test]
        public async Task Delete_Should_Set_ModelStateError_And_RedirectToIndex_On_Failure()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var fishCatch = new FishCatch
            {
                Id = fishCatchId,
                UserId = Guid.NewGuid()
            };

            _fishCatchServiceMock.Setup(service => service.GetFishCatchByIdAsync(fishCatchId))
                .ReturnsAsync(fishCatch);
            _fishCatchServiceMock.Setup(service => service.PermanentDeleteFishCatchAsync(fishCatchId, fishCatch.UserId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(fishCatchId);

            // Assert
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(AdminFishCatchController.Index), redirectResult.ActionName);
        }
    }

}
