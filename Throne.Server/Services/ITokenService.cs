using Throne.Server.Models;

namespace Throne.Server.Services;

public interface ITokenService
{
  string GenerateToken(SignIn user);
}
