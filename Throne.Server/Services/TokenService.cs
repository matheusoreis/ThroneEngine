using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Throne.Server.Models;
using Throne.Shared.Database;

namespace Throne.Server.Services;

public class TokenService(IConfiguration configuration, IDatabase database) : ITokenService
{
    private readonly IDatabase database = database;
    
    public async Task<string> GenerateToken(SignIn user)
    {
        const string query = "SELECT get_roles_by_email(@p_email)";
        NpgsqlParameter[] parameters = [new NpgsqlParameter("@p_email", user.Email)];

        Token? token = await database.ExecuteFunction<Token>(query, parameters);
        
        SymmetricSecurityKey key = new(
            Encoding.UTF8.GetBytes(
                configuration["JwtConfig:Key"] ?? string.Empty
            )
        );
        string? issuer = configuration["JwtConfig:Issuer"];
        string? audience = configuration["JwtConfig:Audience"];

        SigningCredentials signingCredentials = new(
            key, SecurityAlgorithms.HmacSha256
        );
        
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email ?? string.Empty)
        };

        if (token?.IsVip == true)
        {
            claims.Add(new Claim(ClaimTypes.Role, "VIP"));
        }

        if (token?.IsAdmin == true)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        JwtSecurityToken tokenOptions = new(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials
        );

        string? jwtSecurityTokenHandler = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return jwtSecurityTokenHandler;
    }
}