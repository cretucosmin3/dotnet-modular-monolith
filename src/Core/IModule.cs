using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public interface IModule
{
    // Registers services into the DI container
    void RegisterServices(IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder);
    
    // Maps Minimal API routes or module-specific middleware
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}