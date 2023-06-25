using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using chat_server.Controllers.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace chat_server.Controllers.Auth;

[ApiController]
[Route("[controller]/[action]")]
[AllowAnonymous]
public class AuthController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginData loginData)
    {
        var user = await userManager.FindByNameAsync(loginData.Username);

        if (!await UserExists(loginData.Username))
        {
            return Unauthorized("User doesn't exist!");
        }
        
        if (!await IsUserAuthorized(user, loginData.Password))
        {
            return Unauthorized("Wrong password!");
        }

        var authToken = GenerateAuthToken(user);
        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(authToken),
            Expiration = authToken.ValidTo
        });
    }
    
    private async Task<bool> IsUserAuthorized(IdentityUser user, string password)
    {
        return user != null && await userManager.CheckPasswordAsync(user, password);
    }
    
    private JwtSecurityToken GenerateAuthToken(IdentityUser user)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
        var tokenValidityInDays = int.Parse(configuration["JWT:TokenValidityInDays"]);

        return new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(tokenValidityInDays),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }


    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterData registerData)
    {
        if (await UserExists(registerData.Username))
        {
            return BadRequest("User already exists!");
        }

        if (!await TryCreateNewUser(registerData))
        {
            return BadRequest("User creation failed! Please check user details and try again.");
        }

        return Ok("User created successfully!");
    }
    
    private async Task<bool> UserExists(string username)
    {
        var user = await userManager.FindByNameAsync(username);
        return user != null;
    }

    private async Task<bool> TryCreateNewUser(RegisterData registerData)
    {
        var user = new IdentityUser
        {
            Email = registerData.Email,
            UserName = registerData.Username
        };
        var creationResult = await userManager.CreateAsync(user, registerData.Password);
        return creationResult.Succeeded;
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordData resetPasswordData)
    {
        if (!await UserExists(resetPasswordData.Username))
        {
            return BadRequest("User doesn't exist!");
        }

        if (!await TryToChangePassword(resetPasswordData))
        {
            return BadRequest("Something went wrong while changing password!");
        }
        
        return Ok("Password reset successfully!");
    }

    private async Task<bool> TryToChangePassword(ResetPasswordData resetPasswordData)
    {
        var user = await userManager.FindByNameAsync(resetPasswordData.Username);
        var result = await userManager.ChangePasswordAsync(user, resetPasswordData.CurrentPassword, 
            resetPasswordData.NewPassword);
        return result.Succeeded;
    }
}