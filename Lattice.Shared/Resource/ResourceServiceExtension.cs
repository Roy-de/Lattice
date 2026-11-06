using Lattice.Shared.DesignSystem.Colors;
using Lattice.Shared.DesignSystem.Icons;
using Lattice.Shared.DesignSystem.Semantics;
using Lattice.Shared.DesignSystem.Typography;
using Lattice.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lattice.Shared.Resource;

public static class ResourceServiceExtension
{
    public static IServiceCollection AddLatticeResources(this IServiceCollection services)
    {
        services.AddSingleton<ColorInitializer>();
        services.AddSingleton<IResourceLoader, EmbeddedResourceLoader>();
        services.AddSingleton<FontInitializer>();
        
        services.AddScoped<FontRegistry>();
        
        services.AddSingleton<TypographyScssGeneratorOptions>();
        services.AddSingleton<TypographyCssGenerator>();
        services.AddScoped<TypographyCssRegistry>();
        
        services.AddSingleton<IconRegistry>();
        
        services.AddSingleton<TokenInitializer>();
        services.AddSingleton<TokenRegistry>();
        services.AddSingleton<TokenResolver>();
        services.AddSingleton<TokenCssGenerator>();
        services.AddHostedService<LatticeStartupService>();
        return services;
    }
}