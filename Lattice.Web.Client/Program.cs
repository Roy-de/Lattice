using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Lattice.Shared.Services;
using Lattice.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the Lattice.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();