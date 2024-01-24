using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Taster.Application.Exceptions;
using Taster.Application.Interfaces;
using Taster.Application.Logic.Abstractions;
using Taster.Domain.Entities;

namespace Taster.Application.Logic.User
{
    public static class LoginCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
        public class Result
        {
            public required int UserId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IPasswordManager passwordManager;

            public Handler(IApplicationDbContext applicationDbContext, ICurrentUserProvider currentUserProvider, IPasswordManager passwordManager) : base(applicationDbContext, currentUserProvider)
            {
                this.passwordManager = passwordManager;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
               var user = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
                if(user != null)
                {
                    if(passwordManager.VerifyPassword(user.HashedPassword, request.Password))
                    {
                        return new Result()
                        {
                            UserId = user.Id
                        };
                    }
                }

                throw new ErrorException("Invalid login or password");
            }
        }

        public class Validator : AbstractValidator<Request> 
        {
            public Validator() 
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Email).EmailAddress();

                RuleFor(x => x.Password).NotEmpty();
            }
        }
    }
}
