using Taster.Application.Interfaces;
using Taster.Infrastructure.Auth;

namespace Taster.Api.Application.Auth
{
    public class JwtAuthenticationDataProvider : IAuthenticationDataProvider
    {
        private readonly JwtManager jwtManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public JwtAuthenticationDataProvider(JwtManager jwtManager, IHttpContextAccessor httpContextAccessor)
        {
            this.jwtManager = jwtManager;
            this.httpContextAccessor = httpContextAccessor;
        }
        public int? GetUserId()
        {
            var userIdString = GetClaimValue(JwtManager.UserIdClaim);
            if(int.TryParse(userIdString, out int userId)) 
            {
                return userId;
            }

            return null;
        }

        private string? GetTokenFromCookie()
        {
            return httpContextAccessor.HttpContext?.Request.Cookies[CookieSettings.CookieName];
        }

        private string? GetTokenFromHeader()
        {
            var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if(string.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }

            var splited = authorizationHeader.Split(' ');
            if(splited.Length > 1 && splited[0] == "Bearer") 
            {
                return splited[1];
            }

            return null;
        }

        public string? GetClaimValue(string claimType)
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                token = GetTokenFromCookie();
            }

            if(!string.IsNullOrWhiteSpace(token) && jwtManager.ValidateToken(token)) 
            {
                return jwtManager.GetClaim(token, claimType);
            }

            return null;
        }
    }
}
