using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgt.BLL.DTOs.Response
{
    public class UserReturnDto
    {
       public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
    }
}
