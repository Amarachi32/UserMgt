using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Configurations.mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, RegisterDto>().ReverseMap();
            CreateMap<UpdateRequestDto, AppUser>();
            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<AppUser, UpdateRequestDto>();
        }
    }
}
