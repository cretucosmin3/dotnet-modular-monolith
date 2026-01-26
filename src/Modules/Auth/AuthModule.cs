using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Core;

namespace Auth;

public class AuthModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder)
    {
        // Register Controllers
        mvcBuilder.AddApplicationPart(typeof(AuthModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // You can still map Minimal API endpoints here if you prefer them over controllers
        endpoints.MapGet("/auth/status", () => "Auth module is online");
    }
}