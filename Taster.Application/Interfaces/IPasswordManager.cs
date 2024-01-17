namespace Taster.Application.Interfaces
{
    public interface IPasswordManager
    {
        string HashPassword(string password);

        bool VerifyPassword(string hash, string password);
    }
}
