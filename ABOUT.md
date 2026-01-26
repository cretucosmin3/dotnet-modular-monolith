Documentation: dotnet-modular-monolith

This document outlines the architecture for a Distributed Modular Monolith. This pattern provides the developer experience of a single-repo monolith while allowing for "Flavor-based" builds that can be deployed as independent, specialized services.

---

1. Architectural Strategy

The project is structured to enforce strict boundaries using four distinct layers. This prevents "spaghetti code" and allows the compiler to physically exclude unused modules during specialized builds.
Layer Responsibilities



Layer,Project,Role,Dependencies
Core,MyProject.Core,"Contracts, IModule interface, and Domain abstractions.",None
Common,MyProject.Common,"Shared Infrastructure: AppDbContext, Repositories, and Utilities.",Core
Modules,MyProject.Modules.*,"Business logic (Auth, Sales, Analytics). Contains Controllers and Services.","Core, Common"
Main,MyProject.Main,"The Host (Web API). Handles orchestration and ""Flavor"" configuration.","Core, Common, Modules"

---

2. Core Implementation

The IModule interface is the "handshake" between the Main host and the independent projects.

```csharp
namespace MyProject.Core;

public interface IModule
{
    // Registers services into the DI container
    void RegisterServices(IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder);
    
    // Maps Minimal API routes or module-specific middleware
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
```

3. Module Configuration (The "Flavor" System)

The Main.csproj file uses MSBuild properties to conditionally include or exclude modules at compile-time. This ensures that an "Analytics" instance does not contain the binaries for "Auth" or "Sales".

```XML
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ActiveModules Condition="'$(ActiveModules)' == ''">Auth;Sales;Analytics</ActiveModules>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyProject.Core\MyProject.Core.csproj" />
    <ProjectReference Include="..\MyProject.Common\MyProject.Common.csproj" />

    <ProjectReference Include="..\MyProject.Auth\MyProject.Auth.csproj" 
                      Condition="$(ActiveModules.Contains('Auth'))" />
    <ProjectReference Include="..\MyProject.Sales\MyProject.Sales.csproj" 
                      Condition="$(ActiveModules.Contains('Sales'))" />
    <ProjectReference Include="..\MyProject.Analytics\MyProject.Analytics.csproj" 
                      Condition="$(ActiveModules.Contains('Analytics'))" />
  </ItemGroup>
</Project>
```

4. Bootstrapping & Discovery (Program.cs)

The Main project uses reflection to find and initialize whatever modules were included in the current build.

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Setup MVC and capture the builder
var mvcBuilder = builder.Services.AddControllers();

// 2. Discover modules compiled into the current execution context
var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => typeof(IModule).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

var modules = moduleTypes.Select(t => (IModule)Activator.CreateInstance(t)!).ToList();

// 3. Register Module Logic
foreach (var module in modules)
{
    // AddApplicationPart inside RegisterServices allows controllers to be found
    module.RegisterServices(builder.Services, builder.Configuration, mvcBuilder);
}

var app = builder.Build();

// 4. Map Module Routes
foreach (var module in modules)
{
    module.MapEndpoints(app);
}

app.MapControllers();
app.Run();
```

---

5. Deployment Commands

You can generate different "Flavors" of your binary using the CLI. This is ideal for load distributors or internal-only tools.

    Standard Build (Full Monolith): dotnet build

    Analytics-Only Instance: dotnet publish -c Release /p:ActiveModules="Analytics"

    Security & Sales Instance: dotnet publish -c Release /p:ActiveModules="Auth;Sales"