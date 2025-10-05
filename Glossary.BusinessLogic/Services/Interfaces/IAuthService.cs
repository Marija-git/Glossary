using Microsoft.AspNetCore.Identity;

namespace Glossary.BusinessLogic.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> Register(string username, string password,string email);
        Task<string> Login(string username, string password);
    }
}
