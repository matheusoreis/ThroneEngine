using Throne.Server.Configurations;
using Throne.Server.Services;
using Throne.Server.Utils;
using Throne.Server.Websocket;
using Throne.Server.Websocket.Core;
using Throne.Server.Websocket.Core.Memory;
using Throne.Shared.VersionsChecker;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSingleton<IMemoryManager, MemoryManager>();
builder.Services.AddTransient<IDatabase, Database>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddTransient(_ => new VersionChecker(
    Constants.MajorVersion, Constants.MinorVersion, Constants.PatchVersion)
);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSocketHandling();
app.MapControllers();

IServiceProvider serviceProvider = ((IApplicationBuilder)app).ApplicationServices;
ServiceLocator.SetServiceProvider(serviceProvider);

app.Run();