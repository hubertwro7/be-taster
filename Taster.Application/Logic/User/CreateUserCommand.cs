using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Taster.Application.Exceptions;
using Taster.Application.Interfaces;
using Taster.Application.Logic.Abstractions;

namespace Taster.Application.Logic.User
{
    public static class CreateUserCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
            public required string Username { get; set; }
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
                var userExists = await applicationDbContext.Users.AnyAsync(x => x.Email == request.Email);

                if (userExists)
                {
                    throw new ErrorException("User with this email already exists");
                }

                var utcNow = DateTime.UtcNow;
                var user = new Domain.Entities.User()
                {
                    RegisterDate = utcNow,
                    Email = request.Email,
                    HashedPassword = passwordManager.HashPassword(request.Password),
                    Username = request.Username,
                };

                applicationDbContext.Users.Add(user);

                await applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                {
                    UserId = user.Id,
                };
            }
        }

        public class Validator : AbstractValidator<Request> 
        {
            public Validator() 
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Email).EmailAddress();
                RuleFor(x => x.Email).MaximumLength(100);

                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Password).MaximumLength(50);
                RuleFor(x => x.Password).MinimumLength(8);
                RuleFor(x => x.Password).Matches(@"[A-Z]+");
                RuleFor(x => x.Password).Matches(@"[a-z]+");
                RuleFor(x => x.Password).Matches(@"[0-9]+");

                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Username).MaximumLength(20);
            }
        }
    }
}
