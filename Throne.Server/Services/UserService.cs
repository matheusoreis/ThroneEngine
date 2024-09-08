namespace Throne.Server.Services;

public class UserService : IUserService
{
    private readonly Dictionary<string, string> users = [];

    public Task<bool> SignIn(string username, string password)
    {
        if (users.TryGetValue(username, out string? storedPassword) && storedPassword == password)
            return Task.FromResult(true);

        return Task.FromResult(false);
    }

    public Task<bool> SignUp(string username, string password)
    {
        if (users.ContainsKey(username)) return Task.FromResult(false);

        users[username] = password;
        return Task.FromResult(true);
    }
}