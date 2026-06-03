using JWT_Authentication.Dbcontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using JWT_Authentication.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Read secret from configuration (must match AuthController)
var secret = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(secret))
{
    throw new InvalidOperationException("JWT signing key (Jwt:Key) is not configured.");
}

var keyBytes = Encoding.UTF8.GetBytes(secret);
if (keyBytes.Length < 32)
{
    throw new InvalidOperationException("JWT signing key must be at least 32 bytes (256 bits).");
}

var signingKey = new SymmetricSecurityKey(keyBytes);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = signingKey
        };
});

// Register the signing key for other components if needed
builder.Services.AddSingleton<SecurityKey>(signingKey);

// Register the auth service that encapsulates business logic for JWT
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();
