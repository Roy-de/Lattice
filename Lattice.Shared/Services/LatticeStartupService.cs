using Lattice.Shared.DesignSystem.Colors;
using Lattice.Shared.DesignSystem.Semantics;
using Lattice.Shared.DesignSystem.Typography;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.Services;

using Microsoft.Extensions.Hosting;

public sealed class LatticeStartupService(
    ColorInitializer colorInitializer,
    FontInitializer fontInitializer,
    TokenInitializer tokenInitializer,
    ILogger<LatticeStartupService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Lattice resource initialization");

        await colorInitializer.InitializeAsync();
        await fontInitializer.InitializeAsync(cancellationToken);
        await tokenInitializer.InitializeAsync(cancellationToken);
        logger.LogInformation("Lattice resource initialization completed");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}