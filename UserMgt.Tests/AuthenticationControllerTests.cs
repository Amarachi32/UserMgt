using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.BLL.Interface;
using UserMgt.Controllers;

namespace UserMgt.Tests
{
    public class AuthenticationControllerTests
    {
        private AuthenticationController _authenticationController;
        private IAuthService _authService;

        public AuthenticationControllerTests()
        {
            // Create fake instances
            _authService = A.Fake<IAuthService>();
            //var logger = A.Fake<ILogger<AuthenticationController>>();


            _authenticationController = new AuthenticationController(_authService);
        }

        [Fact]
        public async Task RegisterUser_ValidModel_ReturnsCreated()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "admin@gmail.com",
                Password = "@Admin123",
            };

            A.CallTo(() => _authService.RegisterUserAsync(A<RegisterDto>._))
             .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _authenticationController.RegisterUser(registerDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.Equal(nameof(AuthenticationController.RegisterUser), createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task Authenticate_ValidModel_ReturnsAuthResponse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@gmail.com",
                Password = "@Admin123",
            };

            var authResponse = new AuthResponse
            {
                Token = "ValidAuthToken",
                LoginStatus = LoginStatus.LoginSuccessful,
                Message = "Login Successful"
            };
            A.CallTo(() => _authService.UserLogin(A<LoginDto>._))
                .Returns(authResponse);

            // Act
            var result = await _authenticationController.Authenticate(loginDto);

            // Assert
            Assert.IsType<AuthResponse>(result);
            Assert.Same(authResponse, result);
        }

        [Fact]
        public async Task ChangePassword_ValidInput_ReturnsRedirectToActionResult()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                Email = "user@example.com",
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            A.CallTo(() => _authService.ChangePasswordAsync(changePasswordDto))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _authenticationController.ChangePassword(changePasswordDto);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.Equal("ChangePasswordConfirmation", redirectResult.ActionName);
        }

        [Fact]
        public async Task ChangePassword_InvalidInput_ReturnsBadRequestWithModelStateErrors()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                Email = "user@example.com",
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            // Configure the AuthService to return a failure result
            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "ErrorCode1", Description = "Error Description 1" },
                new IdentityError { Code = "ErrorCode2", Description = "Error Description 2" }
            };
            var identityResult = IdentityResult.Failed(identityErrors.ToArray());

            A.CallTo(() => _authService.ChangePasswordAsync(changePasswordDto))
                .Returns(Task.FromResult(identityResult));


            // Act
            var result = await _authenticationController.ChangePassword(changePasswordDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            var modelStateErrors = badRequestResult.Value as SerializableError;

            // Assert that model state contains expected errors
            Assert.True(modelStateErrors.ContainsKey("ErrorCode1"));
            Assert.True(modelStateErrors.ContainsKey("ErrorCode2"));
        }
    }
    }
