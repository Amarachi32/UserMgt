using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgt.DAL.Entities
{
    public class AppUser : IdentityUser
    {
        public Gender Gender { get; set; }
        public bool IsUserActive { get; set; } = true;
    }

    public enum Gender
    {
        Male, Female
    }
}
