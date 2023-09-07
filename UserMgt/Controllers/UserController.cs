using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.BLL.Interface;

namespace UserMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPut("updateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateRequestDto model)
        {

            var result = await _userService.UpdateUserAsync(userId, model);

            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "user details updated successfully" });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User Update Unsucessful")]
        [SwaggerOperation(Summary = "partial update", Description = "request for partial updates")]
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PartialUpdateUser(string userId, JsonPatchDocument<UpdateRequestDto> patchDoc)
        {
            var result = await _userService.PartialUpdateUserAsync(userId, patchDoc);

            if (!result)
            {
                return BadRequest("fail to update");
            }

            return Ok("updated successfully");
        }


        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User profile not found.")]
        [SwaggerOperation(Summary = "Get user profile", Description = "Retrieves the user's profile information.")]
        [HttpGet("profile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID not found");
            }

            var userProfile = await _userService.GetUserProfileAsync(userId);

            if (userProfile == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(userProfile);
        }


        [HttpGet("getUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("getUser")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteUser")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "failed to delete.")]
        [SwaggerOperation(Summary = "Delete USer Account")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deletedUser = await _userService.DeleteAsync(id);

            if (!deletedUser)
            {
                return NotFound("User not found, try again");
            }
            return Ok(new { message = "User deleted successfully" });
        }


        [HttpDelete("deactivate")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status404NotFound, "deactivation failed.")]
        [SwaggerOperation(Summary = "Deactivate user account", Description = "Deactivates the user's account.")]
        public async Task<IActionResult> DeactivateAccount(string userId)
        {
            var deactivated = await _userService.DeactivateAsync(userId);

            if (!deactivated)
            {
                return NotFound("User not found or account already deactivated.");
            }

            return Ok(new { message = "User account deactivated successfully." });
        }
    }
}
