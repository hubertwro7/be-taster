using Taster.Domain.Common;

namespace Taster.Domain.Entities
{
    public class User : DomainEntity
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string HashedPassword { get; set; }
        public DateTimeOffset RegisterDate { get; set; }
    }
}
