using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Controllers;
using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    using FinFinder.Web.ViewModels.FishCatch;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [TestFixture]
    public class FishCatchControllerTests
    {
        private Mock<IFishCatchService> _fishCatchServiceMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IRepository<FishCatch, Guid>> _fishCatchRepositoryMock;
        private FishCatchController _controller;

        [SetUp]
        public void SetUp()
        {
            // Initialize mocks
            _fishCatchServiceMock = new Mock<IFishCatchService>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
            _fishCatchRepositoryMock = new Mock<IRepository<FishCatch, Guid>>();

            // Create the controller with mocked dependencies
            _controller = new FishCatchController(
                _fishCatchServiceMock.Object,
                _userManagerMock.Object,
                _fishCatchRepositoryMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task Index_Should_Return_View_With_Model()
        {
            // Arrange
            var viewModel = new FishCatchFilterViewModel();
            _fishCatchServiceMock.Setup(s => s.GetFilteredFishCatchesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(viewModel);

            // Act
            var result = await _controller.Index("DatePosted", "search");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Create_Get_Should_Return_View_With_Model()
        {
            // Arrange
            var createViewModel = new FishCatchCreateViewModel();
            _fishCatchServiceMock.Setup(s => s.PrepareCreateViewModelAsync()).ReturnsAsync(createViewModel);

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(createViewModel, viewResult.Model);
        }

        [Test]
        public async Task Create_Post_Should_RedirectToIndex_On_Success()
        {
            // Arrange
            var createModel = new FishCatchCreateViewModel();
            _fishCatchServiceMock.Setup(s => s.CreateFishCatchAsync(createModel, It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(createModel);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [Test]
        public async Task Create_Post_Should_Return_View_If_ModelState_Is_Invalid()
        {
            // Arrange
            var mockModel = new FishCatchCreateViewModel
            {
                Species = "Bass",
                Description = "A big fish",
                Weight = 10.5,
                Length = 25.3
            };

            var prepareModel = new FishCatchCreateViewModel
            {
                FishingTechniques = new List<FishingTechnique>
        {
            new FishingTechnique { Id = Guid.NewGuid(), Name = "Fly Fishing" },
            new FishingTechnique { Id = Guid.NewGuid(), Name = "Trolling" }
        }
            };

            _fishCatchServiceMock
                .Setup(service => service.PrepareCreateViewModelAsync())
                .ReturnsAsync(prepareModel);

            _controller.ModelState.AddModelError("Species", "Required");

            // Act
            var result = await _controller.Create(mockModel);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            var returnedModel = viewResult.Model as FishCatchCreateViewModel;

            Assert.NotNull(returnedModel);
            Assert.AreEqual(prepareModel.FishingTechniques.Count, returnedModel.FishingTechniques.Count);
            Assert.AreEqual(prepareModel.FishingTechniques[0].Name, returnedModel.FishingTechniques[0].Name);
        }




        [Test]
        public async Task Edit_Get_Should_Return_View_With_Model()
        {
            // Arrange
            var editModel = new FishCatchEditViewModel();
            _fishCatchServiceMock.Setup(s => s.PrepareEditViewModelAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(editModel);

            // Act
            var result = await _controller.Edit(Guid.NewGuid());

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(editModel, viewResult.Model);
        }
        


        [Test]
        public async Task Edit_Post_Should_Return_View_If_ModelState_Is_Invalid()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var editModel = new FishCatchEditViewModel
            {
                Id = fishCatchId,
                Species = "Invalid Species", // Example data
                FishingTechniques = new List<FishingTechnique>(),
                ExistingPhotos = new List<Photo>(),
            };

            _fishCatchServiceMock
                .Setup(service => service.PrepareEditViewModelAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new FishCatchEditViewModel
                {
                    Id = fishCatchId,
                    Species = "Mock Species",
                    FishingTechniques = new List<FishingTechnique>
                    {
                new FishingTechnique { Id = Guid.NewGuid(), Name = "Mock Technique" }
                    },
                    ExistingPhotos = new List<Photo>
                    {
                new Photo { Id = Guid.NewGuid(), Url = "/images/mock.jpg" }
                    }
                });

            // Mocking the user context
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

            _controller.ModelState.AddModelError("Species", "Required");

            // Act
            var result = await _controller.Edit(fishCatchId, editModel);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;

            // Ensure the same model is returned
            Assert.AreEqual(editModel, viewResult.Model);
        }


        [Test]
        public async Task Details_Should_Return_View_With_Model()
        {
            // Arrange
            var detailsModel = new FishCatchDetailsViewModel();
            _fishCatchServiceMock.Setup(s => s.GetFishCatchDetailsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(detailsModel);

            // Act
            var result = await _controller.Details(Guid.NewGuid());

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(detailsModel, viewResult.Model);
        }

        [Test]
        public async Task AddToFavorites_Should_RedirectToDetails_On_Success()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            _fishCatchServiceMock.Setup(s => s.AddToFavoritesAsync(fishCatchId, It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.AddToFavorites(fishCatchId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
        }

        [Test]
        public async Task SoftDelete_Should_RedirectToIndex_On_Success()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            _fishCatchServiceMock.Setup(s => s.SoftDeleteFishCatchAsync(fishCatchId, It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.SoftDelete(fishCatchId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [Test]
        public async Task PermanentDelete_Should_RedirectToIndex_On_Success()
        {
            // Arrange
            var fishCatchId = Guid.NewGuid();
            _fishCatchServiceMock.Setup(s => s.PermanentDeleteFishCatchAsync(fishCatchId, It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.PermanentDelete(fishCatchId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }
    }

}
