using Microsoft.EntityFrameworkCore;
using Taster.Domain.Entities;

namespace Taster.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
