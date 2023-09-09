using FakeItEasy;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.BLL.Interface;
using UserMgt.Controllers;

namespace UserMgt.Tests
{
    public class UserControllerTests
    {
        private UserController _userController;
        private IUserService _userService;

        public UserControllerTests()
        {
            _userService = A.Fake<IUserService>();

            // Initialize the controller with fake dependencies
            _userController = new UserController(_userService);
        }

        [Fact]
        public async Task UpdateUser_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var userId = "123";
            var updateRequestDto = new UpdateRequestDto();
            A.CallTo(() => _userService.UpdateUserAsync(userId, updateRequestDto))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _userController.UpdateUser(userId, updateRequestDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("user details updated successfully", okResult.Value);
        }

        [Fact]
        public async Task PartialUpdateUser_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var userId = "123";
            var patchDoc = new JsonPatchDocument<UpdateRequestDto>();

            // Configure the UserService to return a success result
            A.CallTo(() => _userService.PartialUpdateUserAsync(userId, patchDoc))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _userController.PartialUpdateUser(userId, patchDoc);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("updated successfully", okResult.Value);
        }

        [Fact]
        public async Task GetUserProfile_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var userId = "123";

            // Configure the UserService to return a user profile
            var userProfile = new ProfileResponse { Username = "testuser", Email = "testuser@example.com" };
            A.CallTo(() => _userService.GetUserProfileAsync(userId))
                .Returns(Task.FromResult(userProfile));

            // Act
            var result = await _userController.GetUserProfile(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(userProfile, okResult.Value);
        }

        [Fact]
        public async Task GetUserProfile_InvalidInput_ReturnsBadRequestResult()
        {
            // Arrange
            var userId = "123";

            // Configure the UserService to return a null profile (user not found)
            A.CallTo(() => _userService.GetUserProfileAsync(userId))
                .Returns(Task.FromResult<ProfileResponse>(null));

            // Act
            var result = await _userController.GetUserProfile(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

