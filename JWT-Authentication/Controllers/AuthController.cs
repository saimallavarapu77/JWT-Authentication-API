using JWT_Authentication.Dbcontext;
using JWT_Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // REGISTER
    [HttpPost("register")]
    public IActionResult Register(User user)
    {
        var existingUser = _context.Users
            .FirstOrDefault(x => x.Email == user.Email);

        if (existingUser != null)
        {
            return BadRequest("User already exists");
        }

        _context.Users.Add(user);

        _context.SaveChanges();

        return Ok("User Registered Successfully");
    }

    // LOGIN
    [HttpPost("login")]
    public IActionResult Login(User loginUser)
    {
        var user = _context.Users.FirstOrDefault(x =>
            x.Email == loginUser.Email &&
            x.Password == loginUser.Password);

        if (user == null)
        {
            return Unauthorized("Invalid Email or Password");
        }

        // CLAIMS
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "User")
        };

        var key = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(
        _config["Jwt:Key"]));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        var jwtToken = new JwtSecurityTokenHandler()
            .WriteToken(token);
        return Ok(new
        {
            token = jwtToken
        });
    }

    // PROTECTED API
    [Authorize]
    [HttpGet("data")]
    public IActionResult GetData()
    {
        return Ok("Protected Data");
    }
}