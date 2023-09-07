using System.ComponentModel.DataAnnotations;

namespace UserMgt.BLL.DTOs.Request
{
    public class ChangePasswordDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        public string OldPassword { get; set; }
    }

}
