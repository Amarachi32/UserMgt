using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserMgt.BLL.Interface;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Services
{
    public class JwtAuthenticator : IJWTAuthenticator
    {
        private readonly IConfiguration _configuration;

        public JwtAuthenticator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<JwtToken> GenerateJwtToken(AppUser user, string expires = null, List<Claim> additionalClaims = null)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();
            string jwtConfig = _configuration.GetSection("JwtConfig:Secret").Value;

            var key = Encoding.ASCII.GetBytes(jwtConfig);

            IdentityOptions _options = new();
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName),
                new Claim(_options.ClaimsIdentity.RoleClaimType, user.Id)
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }
            string issuer = _configuration.GetSection("JwtConfig:Issuer").Value;
            string audience = _configuration.GetSection("JwtConfig:Audience").Value;
            string expire = _configuration.GetSection("JwtConfig:Expires").Value;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = string.IsNullOrWhiteSpace(expires)
                    ? DateTime.Now.AddHours(double.Parse(expire))
                    : DateTime.Now.AddMinutes(double.Parse(expires)),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return new JwtToken
            {
                Token = jwtToken,
                Issued = DateTime.Now,
                Expires = tokenDescriptor.Expires
            };
        }
    }
}
