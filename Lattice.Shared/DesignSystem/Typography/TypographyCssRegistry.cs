using Lattice.Shared.DesignSystem.Typography.Model;

namespace Lattice.Shared.DesignSystem.Typography;
using Microsoft.JSInterop;


public sealed class TypographyCssRegistry(
    IJSRuntime jsRuntime,
    TypographyCssGenerator generator)
{
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);
    
    public async Task ApplyAsync(
        FontTheme theme,
        CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);

        try
        {
            _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                ct,
                "./_content/Lattice.Shared/js/typography-loader.js");

            var css = generator.Generate(theme);

            await _module.InvokeVoidAsync(
                "applyTypographyCss",
                ct,
                css,
                theme.Name);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }

        _lock.Dispose();
    }
}
