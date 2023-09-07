using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Interface
{
    public interface IJWTAuthenticator
    {
         Task<JwtToken> GenerateJwtToken(AppUser user, string expires = null, List<Claim> additionalClaims = null);
    }
        
}
