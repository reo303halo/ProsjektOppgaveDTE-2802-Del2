using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ProsjektOppgaveWebAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = CreateToken(authClaims);
            await _userManager.UpdateAsync(user);
            
            return Ok(new
            {
                success = true,
                token = new JwtSecurityTokenHandler().WriteToken(token),
            });
        }
        return Unauthorized();
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.username);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "User already exists!" });
        }
        IdentityUser user = new()
        {
            Email = model.email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.username
        };
        var result = await _userManager.CreateAsync(user, model.password);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "User creation failed! Please check user details and try again." });
        }
        return Ok(new Response { Success = true, Message = "User created successfully!" });
    }
    
    
    private JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtOptions:Key"]));
        _ = int.TryParse(_configuration["JwtOptions:ExpirationHours"], out int tokenValidityInHours);
        
        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddHours(tokenValidityInHours),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }


}
