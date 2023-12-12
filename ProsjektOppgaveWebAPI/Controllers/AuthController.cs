using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using ProsjektOppgaveWebAPI.Services.AuthServices;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ProsjektOppgaveWebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    //private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, UserManager<IdentityUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(LoginViewModel user)
    {
        if (await _authService.RegisterUser(user))
        {
            return Ok("Registered Successfully!");
        }
        return BadRequest("Something went wrong...");
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginViewModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        if (!await _authService.Login(user)) return BadRequest();
        
        var identityUser = await _userManager.FindByNameAsync(user.Username);
        
        var tokenString = _authService.GenerateTokenString(identityUser);
        return Ok(tokenString);
    }


}
