using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HotelBookingPlatform.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [HttpPost("token")]
    public IActionResult GetToken([FromBody] LoginRequest request)
    {
        if (request.Username != "admin" || request.Password != "Admin123!")
            return Unauthorized(new { message = "Credenciales inválidas." });

        var jwt = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.ExpiryMinutes),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt = token.ValidTo
        });
    }
}

public sealed record LoginRequest(string Username, string Password);

public sealed class JwtOptions
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpiryMinutes { get; set; } = 60;
}
