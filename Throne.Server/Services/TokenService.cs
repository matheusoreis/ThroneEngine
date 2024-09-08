using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Throne.Server.Models;

namespace Throne.Server.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
  private readonly IConfiguration configuration = configuration;

  public string GenerateToken(SignIn user)
  {
    SymmetricSecurityKey? secrectKey = new(
      Encoding.UTF8.GetBytes(
        configuration["JwtConfig:Key"] ?? string.Empty
      )
    );
    string? issuer = configuration["JwtConfig:Issuer"];
    string? audience = configuration["JwtConfig:Audience"];

    SigningCredentials signingCredentials = new(
      secrectKey,
      SecurityAlgorithms.HmacSha256
    );

    JwtSecurityToken tokenOptions = new(
      issuer: issuer,
      audience: audience,
      claims: [
        new Claim(type: ClaimTypes.Name, user.Username ?? string.Empty),
      ],
      expires: DateTime.UtcNow.AddHours(1),
      signingCredentials: signingCredentials
    );

    string? jwtSecurityTokenHandler = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    return jwtSecurityTokenHandler;
  }
}
