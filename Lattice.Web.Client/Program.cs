using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the Lattice.Shared project

// Add logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var host = builder.Build();
await host.RunAsync();
