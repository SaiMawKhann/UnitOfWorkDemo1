using kzy_entities.Entities;

namespace UnitOfWorkDemo1.Interfaces
{
    public interface IAuthService
    {
        Task<string> Authenticate(string username, string password);
        Task<User> Register(string username, string password);
    }
}
