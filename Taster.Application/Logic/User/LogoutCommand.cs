using FluentValidation;
using MediatR;
using Taster.Application.Interfaces;
using Taster.Application.Logic.Abstractions;

namespace Taster.Application.Logic.User
{
    public static class LogoutCommand
    {
        public class Request : IRequest<Result>
        {
        }
        public class Result
        {
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IPasswordManager passwordManager;

            public Handler(IApplicationDbContext applicationDbContext, ICurrentUserProvider currentUserProvider) : base(applicationDbContext, currentUserProvider)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                return new Result()
                {

                };
            }
        }

        public class Validator : AbstractValidator<Request> 
        {
            public Validator() 
            {
            }
        }
    }
}
