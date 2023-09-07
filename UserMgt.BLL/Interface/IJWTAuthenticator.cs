using System.Security.Claims;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Interface
{
    public interface IJWTAuthenticator
    {
        Task<JwtToken> GenerateJwtToken(AppUser user, string expires = null, List<Claim> additionalClaims = null);
    }

}
