using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Interface
{
    public interface IUserService
    {
        Task<ProfileResponse> GetUserProfileAsync(string userId);
        Task<bool> PartialUpdateUserAsync(string userId, JsonPatchDocument<UpdateRequestDto> patchDoc);
        Task<bool> DeleteAsync(string userId);
        Task<bool> UpdateUserAsync(string id, UpdateRequestDto model);
        Task<bool> DeactivateAsync(string userId);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}
