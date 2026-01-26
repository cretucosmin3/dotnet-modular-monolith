using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// A temporary logger for the startup process
using var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
var logger = loggerFactory.CreateLogger("Startup");

// |===============================================|
// ||           1. Standard Services.             ||
// |===============================================|

var mvcBuilder = builder.Services.AddControllers();
builder.Services.AddOpenApi(); 


// |===============================================|
// ||        2. MODULAR DISCOVERY LOGIC           ||
// |===============================================|


// AOT support - The Source Generator provides this method.
var modules = GeneratedModuleDiscovery.GetModules();

if (!modules.Any())
{
    logger.LogWarning("⚠️ No modules were discovered by the generator. Check your project references.");
}

foreach (var module in modules)
{
    logger.LogInformation("📦 Initializing module: {ModuleName}", module.GetType().Name);
    module.RegisterServices(builder.Services, builder.Configuration, mvcBuilder);
}

var app = builder.Build();


// |===============================================|
// ||   3. Configure the HTTP request pipeline.   ||
// |===============================================|

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// |===============================================|
// ||         4. MODULAR ENDPOINT MAPPING.        ||
// |===============================================|

foreach (var module in modules)
{
    logger.LogInformation("🔗 Mapping endpoints for: {ModuleName}", module.GetType().Name);
    module.MapEndpoints(app);
}

app.MapControllers(); 

logger.LogInformation("🚀 Distributed Monolith started successfully.");

app.Run();