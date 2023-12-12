using Microsoft.AspNetCore.Identity;
using ProsjektOppgaveWebAPI.Models.ViewModel;

namespace ProsjektOppgaveWebAPI.Services.AuthServices;

public interface IAuthService
{
    Task<bool> RegisterUser(LoginViewModel user);
    
    Task<bool> Login(LoginViewModel user);
    
    string GenerateTokenString(IdentityUser user);
}