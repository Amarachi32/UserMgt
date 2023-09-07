using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgt.BLL.DTOs.Response
{
    public class AuthResponse
    {
       
        public string Message { get; set; }
        public string Token { get; set; }
        public LoginStatus LoginStatus { get; set; }
    }

    public enum LoginStatus
    {
        LoginFailed,
        LoginSuccessful
    }

    public class TestResponse
    {
        public bool Succeeded { get; set; }
    }
}
