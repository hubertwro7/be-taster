using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Taster.Application.Exceptions;
using Taster.Application.Interfaces;
using Taster.Domain.Entities;

namespace Taster.Application.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IAuthenticationDataProvider authenticationDataProvider;
        private readonly IApplicationDbContext applicationDbContext;

        public CurrentUserProvider(IAuthenticationDataProvider authenticationDataProvider, IApplicationDbContext applicationDbContext)
        {
            this.authenticationDataProvider = authenticationDataProvider;
            this.applicationDbContext = applicationDbContext;
        }
        public async Task<User> GetAuthenticatedUserAsync()
        {
            var userId = authenticationDataProvider.GetUserId();
            if(userId == null)
            {
                throw new UnauthorizedException();
            }
            var user = await applicationDbContext.Users.Cacheable().FirstOrDefaultAsync(x => x.Id == userId.Value);
            if(user == null)
            {
                throw new ErrorException("UserDoesNotExist");
            }

            return user;
        }
    }
}
