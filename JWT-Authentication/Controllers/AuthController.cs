using JWT_Authentication.Dbcontext;
using JWT_Authentication.Models;
using JWT_Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    // LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.AuthenticateAsync(request);
        if (!result.Success)
            return Unauthorized(result.Message);

        return Ok(new AuthResponse
        {
            Token = result.Token!,
            ExpiresAt = result.ExpiresAt!.Value
        });
    }

    // PROTECTED API
    [Authorize(Roles = "User")]
    [HttpGet("data")]
    public IActionResult GetData()
    {
        return Ok("Protected Data");
    }
}