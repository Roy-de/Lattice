using Lattice.Shared.DesignSystem.Typography;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Lattice.Shared.Services;
using Lattice.Web.Client.Services;
using Lattice.Shared.Resource;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the Lattice.Shared project

// Add typography services for WebAssembly
builder.Services.AddSingleton<ITypographyCssGenerator, TypographyCssGenerator>();
builder.Services.AddSingleton<ITypographyService, TypographyService>();
builder.Services.AddSingleton<IResourceLoader, WebAssemblyResourceLoader>();

// Add logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);

await builder.Build().RunAsync();