using ProsjektOppgaveDTE_2802.Models.ViewModel;

namespace ProsjektOppgaveDTE_2802.AuthProviders;

public interface IAuthenticationService
{
    Task<RegisterResponse> RegisterUser(LoginViewModel loginViewModel);
    
    Task<LoginResponse> Login(LoginViewModel loginViewModel);
    
    Task Logout();
}