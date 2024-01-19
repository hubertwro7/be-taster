using MediatR;
using Microsoft.AspNetCore.Mvc;
using Taster.Application.Logic.User;

namespace Taster.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        public UsersController(ILogger logger, IMediator mediator) : base(logger, mediator)
        {
        }

        [HttpPost("add")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserCommand.Request model)
        {
            var createUserResult = await mediator.Send(model);
            return Ok(createUserResult);
        }
    }
}
