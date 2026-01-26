using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Core;

namespace Analytics;

public class AnalyticsModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder)
    {
        // Register Controllers
        mvcBuilder.AddApplicationPart(typeof(AnalyticsModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // You can still map Minimal API endpoints here if you prefer them over controllers
        endpoints.MapGet("/analytics/status", () => "Analytics module is online");
    }
}