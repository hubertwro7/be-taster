using Azure.Core;
using MediatR;
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

        public UsersController(ILogger<UsersController> logger, IMediator mediator, IOptions<CookieSettings> cookieSettings, JwtManager jwtManager) : base(logger, mediator)
        {
            this.cookieSettings = cookieSettings != null ? cookieSettings.Value : null;
            this.jwtManager = jwtManager;
        }

        [HttpPost("add")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserCommand.Request model)
        {
            var createUserResult = await mediator.Send(model);
            var token = jwtManager.GenerateUserToken(createUserResult.UserId);
            SetTokenCookie(token);
            return Ok(new JwtToken() { AccessToken = token });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginCommand.Request model)
        {
            var loginResult = await mediator.Send(model);
            var token = jwtManager.GenerateUserToken(loginResult.UserId);
            SetTokenCookie(token);
            return Ok(new JwtToken() { AccessToken = token });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var logoutResult = await mediator.Send(new LogoutCommand.Request());
            DeleteTokenCookie();
            return Ok(logoutResult);
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
