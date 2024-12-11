using FinFinder.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Tests.Controller.Tests
{
    [TestFixture]
    public class ErrorControllerTests
    {
        private ErrorController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new ErrorController();

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public void NotFoundPage_Should_Return_NotFound_View_And_Set_StatusCode_To_404()
        {
            // Act
            var result = _controller.NotFoundPage();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("NotFound", viewResult.ViewName);
            Assert.AreEqual(404, _controller.Response.StatusCode);
        }

        [Test]
        public void InternalServerError_Should_Return_ServerError_View_And_Set_StatusCode_To_500()
        {
            // Act
            var result = _controller.InternalServerError();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("ServerError", viewResult.ViewName);
            Assert.AreEqual(500, _controller.Response.StatusCode);
        }
    }


}
