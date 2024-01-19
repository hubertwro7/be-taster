using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Taster.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger logger;
        protected readonly IMediator mediator;

        public BaseController(ILogger logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }
    }
}
