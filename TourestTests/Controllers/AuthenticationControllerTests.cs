using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tourest.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels.Account;
using Moq;
using Tourest.Data.Entities;

namespace Tourest.Controllers.Tests
{
    [TestClass]
    public class AuthenticationControllerTests
    {
        private Mock<IAccountService> _mockAccountService;
        private AuthenticationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AuthenticationController(null, null, _mockAccountService.Object);

            // Mock HttpContext & Session
            var context = new DefaultHttpContext();
            var session = new Mock<ISession>();
            context.Session = session.Object;

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };
        }

        [TestMethod]
        public async Task Register_EmailExists_RedirectsToLogin()
        {
            // Arrange
            var model = new AuthenticationViewModel
            {
                Register = new RegisterModel
                {
                    Email = "test@example.com",
                    ReturnUrl = "/home"
                }
            };

            _mockAccountService
                .Setup(s => s.CheckEmailAsync(model.Register.Email))
                .ReturnsAsync(new User()); // Email đã tồn tại

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Login", redirect.ActionName);
            Assert.AreEqual("Authentication", redirect.ControllerName);
        }
    }
}