using Throne.Server.Models;

namespace Throne.Server.Services;

public interface ITokenService
{
    Task<string> GenerateToken(SignIn user);
}