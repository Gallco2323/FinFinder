using FinFinder.Data.Models;
using FinFinder.Web.Areas.Admin.Controllers;
using FinFinder.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
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
    public class UserControllerTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole<Guid>>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(roleStoreMock.Object, null, null, null, null);

            _controller = new UserController(_userManagerMock.Object, _roleManagerMock.Object);
        }

        [Test]
        public async Task Index_Should_Return_View_With_Users()
        {
            // Arrange
            var users = new List<ApplicationUser>
    {
        new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1", Email = "user1@example.com" },
        new ApplicationUser { Id = Guid.NewGuid(), UserName = "User2", Email = "user2@example.com" }
    };

            _userManagerMock.Setup(um => um.Users).Returns(users.AsQueryable());

            foreach (var user in users)
            {
                _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Role1", "Role2" });
            }

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<UserViewModel>;
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("User1", model[0].UserName);

            // Assert Roles
            var user1Roles = model[0].Roles.ToList(); // Convert IEnumerable<string> to List<string>
            Assert.AreEqual("Role1", user1Roles[0]); // Assert first role
            Assert.AreEqual(2, user1Roles.Count); // Assert number of roles
        }


        [Test]
        public async Task Edit_Get_Should_Return_View_With_EditUserViewModel()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, UserName = "User1", Email = "user1@example.com" };
            var roles = new List<string> { "Admin" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(roles);
            _roleManagerMock.Setup(rm => rm.Roles).Returns(new List<IdentityRole<Guid>>
        {
            new IdentityRole<Guid>("Admin"),
            new IdentityRole<Guid>("User")
        }.AsQueryable());

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as EditUserViewModel;
            Assert.AreEqual(user.UserName, model.UserName);
            Assert.AreEqual(2, model.AllRoles.Count);
        }

        [Test]
        public async Task Edit_Post_Should_Update_User_And_Roles()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, UserName = "User1", Email = "user1@example.com" };
            var userRoles = new List<string> { "Admin" };
            var model = new EditUserViewModel
            {
                Id = userId,
                UserName = "UpdatedUser",
                Email = "updated@example.com",
                Roles = new List<string> { "User" }
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(userRoles);
            _userManagerMock.Setup(um => um.RemoveFromRolesAsync(user, userRoles.Except(model.Roles).ToList()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRolesAsync(user, model.Roles.Except(userRoles).ToList()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Edit(model);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task Edit_Post_Should_Return_View_If_ModelState_Is_Invalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Required");
            var model = new EditUserViewModel();

            // Act
            var result = await _controller.Edit(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Delete_Should_Delete_User_And_RedirectToIndex()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, UserName = "User1", Email = "user1@example.com" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            _userManagerMock.Verify(um => um.DeleteAsync(user), Times.Once);
        }

        [Test]
        public async Task Delete_Should_Return_NotFound_If_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }

}
