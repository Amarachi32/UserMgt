using Microsoft.AspNetCore.Identity;

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
