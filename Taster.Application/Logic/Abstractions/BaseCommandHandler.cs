using Taster.Application.Interfaces;

namespace Taster.Application.Logic.Abstractions
{
    public abstract class BaseCommandHandler
    {
        protected readonly IApplicationDbContext applicationDbContext;
        protected readonly ICurrentUserProvider currentUserProvider;

        public BaseCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUserProvider currentUserProvider)
        {
            this.applicationDbContext = applicationDbContext;
            this.currentUserProvider = currentUserProvider;
        }
    }
}
