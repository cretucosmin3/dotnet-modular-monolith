using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Core;

namespace Sales;

public class SalesModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder)
    {
        // Register Controllers
        mvcBuilder.AddApplicationPart(typeof(SalesModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // You can still map Minimal API endpoints here if you prefer them over controllers
        endpoints.MapGet("/sales/status", () => "Sales module is online");
    }
}