using EFCoreSecondLevelCacheInterceptor;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Taster.Application.Exceptions;
using Taster.Application.Interfaces;
using Taster.Application.Logic.Abstractions;

namespace Taster.Application.Logic.User
{
    public static class CurrentUserQuery
    {
        public class Request : IRequest<Result>
        {
            
        }
        public class Result
        {
            public required string Username { get; set; }
            public required string Email { get; set; }
        }

        public class Handler : BaseQueryHandler, IRequestHandler<Request, Result>
        {
            private readonly IAuthenticationDataProvider authenticationDataProvider;

            public Handler(IApplicationDbContext applicationDbContext, ICurrentUserProvider currentUserProvider, IAuthenticationDataProvider authenticationDataProvider) : base(applicationDbContext, currentUserProvider)
            {
                this.authenticationDataProvider = authenticationDataProvider;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = authenticationDataProvider.GetUserId();
                if(userId.HasValue)
                {
                    var user = await applicationDbContext.Users.Cacheable().FirstOrDefaultAsync(x => x.Id == userId.Value);
                    if(user != null)
                    {
                        return new Result()
                        {
                            Username = user.Username,
                            Email = user.Email,
                        };
                    }
                }

                throw new UnauthorizedException();
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
