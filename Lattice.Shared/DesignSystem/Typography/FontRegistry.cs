namespace Lattice.Shared.DesignSystem.Typography;

using Resource;
using Microsoft.JSInterop;


public sealed class FontRegistry(IResourceLoader resources, IJSRuntime jsRuntime)
{
    private IJSObjectReference? _module;

    private readonly HashSet<string> _registeredFonts = new(StringComparer.OrdinalIgnoreCase);

    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task RegisterAsync(string family, string path, string weight, string style = "normal", CancellationToken ct = default)
    {
        var key = $"{family}:{weight}:{style}";

        if (_registeredFonts.Contains(key))
            return;

        await _lock.WaitAsync(ct);

        try
        {
            if (_registeredFonts.Contains(key))
                return;

            _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                ct,
                "./_content/Lattice.Shared/js/font-loader.js");

            var resourcePath = $"Fonts/{path}";

            await using var stream =
                await resources.OpenAsync(resourcePath, ct);

            using var memory = new MemoryStream();

            await stream.CopyToAsync(memory, ct);

            var base64 = Convert.ToBase64String(memory.ToArray());

            await _module.InvokeVoidAsync(
                "registerFont",
                ct,
                family,
                base64,
                weight,
                style);

            _registeredFonts.Add(key);
        }
        finally
        {
            _lock.Release();
        }
    }
}