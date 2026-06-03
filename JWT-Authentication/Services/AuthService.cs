using JWT_Authentication.Dbcontext;
using JWT_Authentication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace JWT_Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly SecurityKey _signingKey;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(AppDbContext db, SecurityKey signingKey)
        {
            _db = db;
            _signingKey = signingKey;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<(bool Success, string? Message)> RegisterAsync(RegisterRequest request)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
                return (false, "User already exists");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email
                // Password will be hashed below
            };

            user.Password = _passwordHasher.HashPassword(user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, "User Registered Successfully");
        }

        public async Task<(bool Success, string? Message, string? Token, DateTime? ExpiresAt)> AuthenticateAsync(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
                return (false, "Invalid Email or Password", null, null);

            var verification = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (verification == PasswordVerificationResult.Failed)
                return (false, "Invalid Email or Password", null, null);

            // Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return (true, "Authenticated", jwtToken, expires);
        }
    }
}
