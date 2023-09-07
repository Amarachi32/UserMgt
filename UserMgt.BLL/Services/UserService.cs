using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.BLL.Interface;
using UserMgt.DAL.Context;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateRequestDto> _validator;
        private readonly UserDbContext _userDb;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMapper mapper, IValidator<UpdateRequestDto> validator, UserDbContext userDb)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
            _validator = validator;
            _userDb = userDb;
        }

        public async Task<ProfileResponse> GetUserProfileAsync(string userId)
        {


            var userProfile = await _userDb.Users
                .Where(u => u.Id == userId)
                .Select(u => new ProfileResponse
                {
                    Username = u.UserName,
                    Email = u.Email,

                })
                .FirstOrDefaultAsync();

            return userProfile;
        }


        public async Task<bool> UpdateUserAsync(string id, UpdateRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                throw new ArgumentException($"User not found");
            }
            var updatedUser = _mapper.Map(model, user);

            var result = await _userManager.UpdateAsync(updatedUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update user detail");
            }

            return result.Succeeded;
        }


        public async Task<bool> PartialUpdateUserAsync(string userId, [FromBody] JsonPatchDocument<UpdateRequestDto> patchDoc)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }
            var userUpdateDto = _mapper.Map<UpdateRequestDto>(user);

            patchDoc.ApplyTo(userUpdateDto);

            var validationResult = _validator.Validate(userUpdateDto);

            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }
            _mapper.Map(userUpdateDto, user);
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }


        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var userfetch = await _userManager.FindByIdAsync(userId);

            if (userfetch == null) throw new KeyNotFoundException("User not found");
            var user = _mapper.Map<UserDto>(userfetch);

            return user;
        }


        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return userDtos;
        }


        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            return true;
        }


        public async Task<bool> DeactivateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.IsUserActive = false;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}
