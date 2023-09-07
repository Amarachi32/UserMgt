using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.DTOs.Response
{
    public class UserDto
    {
            public string UserName { get; set; }
            public string Email { get; set; }
            public Gender Gender { get; set; }
            public bool IsActive { get; set; }
        
    }
}
