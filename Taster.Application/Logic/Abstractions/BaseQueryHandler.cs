using Taster.Application.Interfaces;

namespace Taster.Application.Logic.Abstractions
{
    public abstract class BaseQueryHandler
    {
        protected readonly IApplicationDbContext applicationDbContext;
        protected readonly ICurrentUserProvider currentUserProvider;

        public BaseQueryHandler(IApplicationDbContext applicationDbContext, ICurrentUserProvider currentUserProvider)
        {
            this.applicationDbContext = applicationDbContext;
            this.currentUserProvider = currentUserProvider;
        }
    }
}
