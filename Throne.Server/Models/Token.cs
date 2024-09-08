namespace Throne.Server.Models;

public abstract record Token
{
    public bool IsAdmin;
    public bool IsVip;
}