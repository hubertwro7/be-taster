using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Taster.Api.Application.Auth;
using Taster.Api.Application.Response;
using Taster.Application.Logic.User;
using Taster.Infrastructure.Auth;

namespace Taster.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly CookieSettings? cookieSettings;
        private readonly JwtManager jwtManager;
        private readonly IAntiforgery antiForgery;

        public UsersController(ILogger<UsersController> logger, IMediator mediator, IOptions<CookieSettings> cookieSettings, JwtManager jwtManager, IAntiforgery antiForgery) : base(logger, mediator)
        {
            this.cookieSettings = cookieSettings != null ? cookieSettings.Value : null;
            this.jwtManager = jwtManager;
            this.antiForgery = antiForgery;
        }

        [HttpPost("add")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserCommand.Request model)
        {
            var createUserResult = await mediator.Send(model);
            var token = jwtManager.GenerateUserToken(createUserResult.UserId);
            SetTokenCookie(token);
            return Ok(new JwtToken() { AccessToken = token });
        }

        [HttpPost("/auth/login")]
        public async Task<ActionResult> Login([FromBody] LoginCommand.Request model)
        {
            var loginResult = await mediator.Send(model);
            var token = jwtManager.GenerateUserToken(loginResult.UserId);
            SetTokenCookie(token);
            return Ok(new JwtToken() { AccessToken = token });
        }

        [HttpPost("/auth/logout")]
        public async Task<ActionResult> Logout()
        {
            var logoutResult = await mediator.Send(new LogoutCommand.Request());
            DeleteTokenCookie();
            return Ok(logoutResult);
        }

        [HttpGet("current-user")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var data = await mediator.Send(new CurrentUserQuery.Request() { });    
            return Ok(data);
        }

        [HttpGet("anti-forgery-token")]
        public async Task<ActionResult> GetAntiForgeryToken()
        {
            var tokens = antiForgery.GetAndStoreTokens(HttpContext);
            return Ok(tokens.RequestToken);
        }


        private void SetTokenCookie(string token)
        {
            var cookieOption = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(30),
                SameSite = SameSiteMode.Lax,
            };

            if(cookieSettings != null)
            {
                cookieOption = new CookieOptions()
                {
                    HttpOnly = cookieOption.HttpOnly,
                    Expires = cookieOption.Expires,
                    Secure = cookieOption.Secure,
                    SameSite = cookieOption.SameSite
                };
            }

            Response.Cookies.Append(CookieSettings.CookieName, token, cookieOption);
        }

        private void DeleteTokenCookie()
        {
            Response.Cookies.Delete(CookieSettings.CookieName, new CookieOptions()
            {
                HttpOnly = true
            });
        }
    }
}
