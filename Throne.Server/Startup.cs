using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Throne.Server.Services;
using Throne.Server.Websocket;
using Throne.Server.Websocket.Core;
using Throne.Server.Websocket.Core.Memory;
using Throne.Shared.Database;

namespace Throne.Server;

public class Startup(IConfiguration configuration)
{
    private readonly IConfiguration configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Throne API",
                        Version = "v1"
                    }
                );

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                );
            }
        );
        services.AddControllers();
        services.AddSingleton<IMemoryManager, MemoryManager>();
        services.AddSingleton<WSManager>();
        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(
            jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtConfig:Issuer"],
                    ValidAudience = configuration["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["JwtConfig:Key"]!
                        )
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            }
        );
        services.AddTransient<IDatabase, Database>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ITokenService, TokenService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseWebSockets();
        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(async (context, next) =>
        {
            await HandleWebSocketRequest(context);
            await next();
        });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        IServiceProvider serviceProvider = app.ApplicationServices;
        ServiceLocator.SetServiceProvider(serviceProvider);
    }

    private static async Task HandleWebSocketRequest(HttpContext context)
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                string ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                WSManager websocketManager = context.RequestServices.GetRequiredService<WSManager>();
                await websocketManager.HandleWebSocketConnection(webSocket, ip);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await Task.CompletedTask;
        }
    }
}