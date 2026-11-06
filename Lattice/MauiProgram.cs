using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddLatticeResources();

        builder.Services.AddMauiBlazorWebView();
        

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }
}