using System;

namespace Throne.Server.Services;

public interface IUserService
{
  Task<bool> SignUp(string username, string password);
  Task<bool> SignIn(string username, string password);
}