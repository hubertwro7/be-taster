using Microsoft.AspNetCore.Identity;
using Taster.Application.Interfaces;

namespace Taster.Infrastructure.Auth
{
    public class PasswordManager : IPasswordManager
    {
        private readonly IPasswordHasher<DummyUser> passwordHasher;

        public PasswordManager(IPasswordHasher<DummyUser> passwordHasher)
        {
            this.passwordHasher = passwordHasher;
        }

        public string HashPassword(string password)
        {
            return passwordHasher.HashPassword(new DummyUser(), password);
        }

        public bool VerifyPassword(string hash, string password)
        {
            var verificationResult = passwordHasher.VerifyHashedPassword(new DummyUser(), hash, password);
            if(verificationResult == PasswordVerificationResult.Success || verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return true;
            }

            return false;
        }

        public class DummyUser
        {
        }
    }
}
