using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.BLL.Errors;
using UserMgt.BLL.Interface;

namespace UserMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [ValidationFilter]
        [Route(("User"))]
        // [ServiceFilter(typeof(ValidationFilterAttribute))]
        [SwaggerOperation(Summary = "Creates user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "User Created Successfully", Type = typeof(RegisterDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User with provided email already exists", Type = typeof(ErrorDetails))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create user", Type = typeof(ErrorDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorDetails))]

        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto register)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterUserAsync(register);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                var userReturnDto = new UserReturnDto
                {
                    IsSuccessful = true,
                    Message = "User successfully created",
                    //UserId = result.UserId
                };

                return CreatedAtAction(nameof(RegisterUser), userReturnDto);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route(("Auth"))]
        [SwaggerOperation(Summary = "Authenticate and login in a user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "User login successful", Type = typeof(AuthResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid Username and Password", Type = typeof(ErrorDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorDetails))]
        public async Task<AuthResponse> Authenticate([FromBody] LoginDto loginDto)
        {
            AuthResponse response = await _authService.UserLogin(loginDto);
            return response;
        }


        [Authorize]
        [HttpPost]
        [Route(("password"))]
        [SwaggerOperation(Summary = "change user password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var result = await _authService.ChangePasswordAsync(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return RedirectToAction("ChangePasswordConfirmation");
        }



        [HttpGet]
        [Route(("password"))]
        [SwaggerOperation(Summary = "change password confirmed")]
        public IActionResult ChangePasswordConfirmend()
        {
            return Ok("Password changed successfully. Kindly login with your new password");
        }

        [HttpPost]
        [Route(("logout"))]
        [SwaggerOperation(Summary = "log a user out")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            return RedirectToAction("LogoutConfirmation");
        }


        [HttpGet]
        [Route(("logout"))]
        [SwaggerOperation(Summary = "user has beed logged out")]
        public IActionResult LogoutConfirmed()
        {
            return Ok("You have been successfully logged out");
        }

    }
}
