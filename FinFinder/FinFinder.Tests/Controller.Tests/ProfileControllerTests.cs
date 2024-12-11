using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.Controllers;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Profile;
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
    public class ProfileControllerTests
    {
        private ProfileController _controller;
        private Mock<IProfileService> _profileServiceMock;

        [SetUp]
        public void SetUp()
        {
            _profileServiceMock = new Mock<IProfileService>();

            _controller = new ProfileController(_profileServiceMock.Object)
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
        public async Task Edit_Get_Should_Return_View_With_Model_If_User_Exists()
        {
            // Arrange
            var profileModel = new ProfileEditViewModel { UserName = "TestUser" };
            _profileServiceMock.Setup(s => s.GetProfileForEditAsync(It.IsAny<Guid>())).ReturnsAsync(profileModel);

            // Act
            var result = await _controller.Edit();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(profileModel, viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_Should_Return_NotFound_If_User_Not_Exists()
        {
            // Arrange
            _profileServiceMock.Setup(s => s.GetProfileForEditAsync(It.IsAny<Guid>())).ReturnsAsync((ProfileEditViewModel)null);

            // Act
            var result = await _controller.Edit();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
        [Test]
        public async Task Edit_Post_Should_Return_View_If_ModelState_Is_Invalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Required");

            var profileModel = new ProfileEditViewModel
            {
                UserName = "TestUser", // Provide required property values
                Bio = "This is a test bio.",
                ProfilePictureURL = "/images/test-profile.png" // Optional property
            };

            // Act
            var result = await _controller.Edit(profileModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(profileModel, viewResult.Model);
        }


        [Test]
        public async Task Edit_Post_Should_Redirect_To_Home_If_Success()
        {
            // Arrange
            var profileModel = new ProfileEditViewModel { UserName = "TestUser" };
            _profileServiceMock.Setup(s => s.UpdateProfileAsync(It.IsAny<Guid>(), profileModel)).ReturnsAsync(true);

            // Act
            var result = await _controller.Edit(profileModel);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);
        }

        [Test]
        public async Task Edit_Post_Should_Return_View_With_Error_If_Failed_To_Update()
        {
            // Arrange
            var profileModel = new ProfileEditViewModel { UserName = "TestUser" };
            _profileServiceMock.Setup(s => s.UpdateProfileAsync(It.IsAny<Guid>(), profileModel)).ReturnsAsync(false);

            // Act
            var result = await _controller.Edit(profileModel);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(profileModel, viewResult.Model);
            Assert.IsTrue(_controller.ModelState.ContainsKey(string.Empty));
        }

        [Test]
        public async Task Details_Should_Return_View_With_Model_If_User_Exists()
        {
            // Arrange
            var profileDetails = new UserProfileViewModel { UserName = "TestUser" };
            _profileServiceMock.Setup(s => s.GetProfileDetailsAsync(It.IsAny<Guid>())).ReturnsAsync(profileDetails);

            // Act
            var result = await _controller.Details(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(profileDetails, viewResult.Model);
        }

        [Test]
        public async Task Details_Should_Return_NotFound_If_User_Not_Exists()
        {
            // Arrange
            _profileServiceMock.Setup(s => s.GetProfileDetailsAsync(It.IsAny<Guid>())).ReturnsAsync((UserProfileViewModel)null);

            // Act
            var result = await _controller.Details(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task HiddenPosts_Should_Return_View_With_Model_If_User_Is_Authorized()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var hiddenPosts = new List<FishCatchHiddenViewModel>
    {
        new FishCatchHiddenViewModel { Id = Guid.NewGuid(), Species = "Test Fish", LocationName = "Test Location", DateCaught = DateTime.UtcNow }
    };

            // Mock the User identity to match the authorized userId
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

            // Mock the ProfileService response
            _profileServiceMock.Setup(service => service.GetHiddenPostsAsync(userId))
                .ReturnsAsync(hiddenPosts);

            // Act
            var result = await _controller.HiddenPosts(userId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(hiddenPosts, viewResult.Model);
        }


        [Test]
        public async Task HiddenPosts_Should_Return_Unauthorized_If_User_Not_Authorized()
        {
            // Arrange
            var unauthorizedUserId = Guid.NewGuid();
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, unauthorizedUserId.ToString())
        }));

            // Act
            var result = await _controller.HiddenPosts(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Unhide_Should_Redirect_To_HiddenPosts_On_Success()
        {
            // Arrange
            _profileServiceMock.Setup(s => s.UnhidePostAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Unhide(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("HiddenPosts", redirectResult.ActionName);
        }

        [Test]
        public async Task Unhide_Should_Return_Unauthorized_If_Unhide_Fails()
        {
            // Arrange
            _profileServiceMock.Setup(s => s.UnhidePostAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            var result = await _controller.Unhide(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
    }
}
