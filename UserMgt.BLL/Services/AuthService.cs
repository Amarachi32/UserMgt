using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.BLL.Interface;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Services
{
    public class AuthService : IAuthService
    {
            private readonly IMapper _mapper;
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IJWTAuthenticator _jWTAuthenticator;
            private readonly SignInManager<AppUser> _signInManager;
            private AppUser? _user;


            public AuthService(IMapper mapper, UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IJWTAuthenticator jWTAuthenticator)
            {
                _mapper = mapper;
                _userManager = userManager;
                _configuration = configuration;
                _signInManager = signInManager;
                _roleManager = roleManager;
                _jWTAuthenticator = jWTAuthenticator;
             }

            public async Task<IdentityResult> RegisterUserAsync(RegisterDto register)
            {
                _user = await _userManager.FindByEmailAsync(register.Email);
                if (_user != null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User already exist" });
                }
               var newUser = _mapper.Map<AppUser>(register);
               var result = await _userManager.CreateAsync(newUser, register.Password);
                if (result.Succeeded)
                {
                    return result;
                }
                return result;
            }

            public async Task<AuthResponse> UserLogin(LoginDto loginDto)
            {
                _user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (_user == null)
                    return new AuthResponse { Message = "A user with this email address does not exist." };
                var result = (_user != null && await _userManager.CheckPasswordAsync(_user, loginDto.Password));
 
                if (result)
                    {
                        //var additionalClaims = new List<Claim> { new Claim("userType", _user.UserType.GetStringValue()) };
                        JwtToken token = await _jWTAuthenticator.GenerateJwtToken(_user);
                        var authResponse = new AuthResponse()
                        {
                       
                            Token = token.Token.ToString(),
                            LoginStatus = LoginStatus.LoginSuccessful,
                            Message = "Login Successful"
                        };
                        return authResponse;

                }
                return new AuthResponse()
                {
                    LoginStatus = LoginStatus.LoginFailed,
                    Message = "Incorrect Username and password"
                };


            }


        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordDto changePassword)
            {
                var user = await _userManager.FindByEmailAsync(changePassword.Email);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }

                var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
                
                if (!result.Succeeded)
                {
                     return IdentityResult.Failed(new IdentityError { Description = "Invalid Email and Password" });
                }
                 await _signInManager.SignOutAsync();
                 return result;
        }


        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

    }
}

  

