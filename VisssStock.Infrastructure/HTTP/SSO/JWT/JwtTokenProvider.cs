using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using VisssStock.Domain.DataObjects;
using Microsoft.IdentityModel.Tokens;
using VisssStock.Infrastructure;
using VisssStock.Infrastructure.Data;

namespace VisssStock.Infrastructure.HTTP.SSO.JWT
{
    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;


        public JwtTokenProvider(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }


        public string CreateToken(IPrincipal userLogin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userLogin.Identity.Name),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:TokenKeySecret").Value));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(3),
                SigningCredentials = creds,
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
