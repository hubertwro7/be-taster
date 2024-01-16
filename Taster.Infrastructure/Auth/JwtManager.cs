using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Taster.Infrastructure.Auth
{
    public class JwtManager
    {
        private readonly JwtAuthenticationOptions jwtAuthenticationOptions;

        public const string UserIdClaim = "UserId";
        public JwtManager(IOptions<JwtAuthenticationOptions> jwtAuthenticationOptions)
        {
            this.jwtAuthenticationOptions = jwtAuthenticationOptions.Value;
        }

        private SecurityKey GetSecurityKey()
        {
            if(string.IsNullOrWhiteSpace(jwtAuthenticationOptions.Secret))
            {
                throw new ArgumentException("JWT options secret is empty!");
            }
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAuthenticationOptions.Secret));
        }

        private string GenerateTokenWithClaims(IEnumerable<Claim> claims)
        {
            var mySecurityKey = GetSecurityKey();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(jwtAuthenticationOptions.ExpireInDays),
                Issuer = jwtAuthenticationOptions.Issuer,
                Audience = jwtAuthenticationOptions.Audience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateUserToken(int userId)
        {
            var claims = new Claim[]
            {
                new Claim(UserIdClaim, userId.ToString()),
            };
            return GenerateTokenWithClaims(claims);
        }

        public bool ValidateToken(string token)
        {
            if(string.IsNullOrEmpty(token)) return false;
            var mySecurityKey = GetSecurityKey();

            var tokenHandler = new JwtSecurityTokenHandler();   
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtAuthenticationOptions.Issuer,
                    ValidAudience = jwtAuthenticationOptions.Audience,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            { return false; }
            return true;
        }
    }
}
