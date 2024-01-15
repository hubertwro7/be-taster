using Taster.Domain.Entities;

namespace Taster.Application.Interfaces
{
    public interface ICurrentUserProvider
    {
        Task<User> GetAuthenticatedUserAsync();
    }
}
