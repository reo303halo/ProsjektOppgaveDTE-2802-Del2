using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProsjektOppgaveWebAPI.Models.ViewModel;

namespace ProsjektOppgaveWebAPI.Services.AuthServices;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<bool> RegisterUser(LoginViewModel user)
    {
        var identityUser = new IdentityUser
        {
            UserName = user.Username,
            Email = user.Username
        };

        var result = await _userManager.CreateAsync(identityUser, user.Password);
        return result.Succeeded;
    }

    public async Task<bool> Login(LoginViewModel user)
    {
        var identityUser = await _userManager.FindByEmailAsync(user.Username);
        if (identityUser is null)
        {
            return false;
        }

        return await _userManager.CheckPasswordAsync(identityUser, user.Password);
    }

    public string GenerateTokenString(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        
        var jwtKey = _config.GetSection("Jwt:Key").Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        
        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(48),
            issuer: _config.GetSection("Jwt:Issuer").Value,
            audience: _config.GetSection("Jwt:Audience").Value,
            signingCredentials: signingCredentials
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return tokenString;
    }
}