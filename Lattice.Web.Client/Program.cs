using Lattice.Shared.DesignSystem.Typography;
using Lattice.Shared.DesignSystem.Colors;
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
builder.Services.AddSingleton<ColorPaletteInitializer>();
builder.Services.AddSingleton<IColorPaletteCssGenerator, ColorPaletteCssGenerator>();
builder.Services.AddSingleton<IColorPaletteService, ColorPaletteService>();

// Add logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var host = builder.Build();
await host.Services.GetRequiredService<ColorPaletteInitializer>().InitializeAsync();
await host.RunAsync();
