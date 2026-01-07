using InternalProcessMgmt.Models;
namespace InternalProcessMgmt.Services
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string username, string password);
        Task<User?> RegisterAsync(string username, string email, string password, string roleName);
    }
}
