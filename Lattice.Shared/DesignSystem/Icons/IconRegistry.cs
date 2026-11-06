using System.Collections.Concurrent;
using System.Xml.Linq;
using Lattice.Shared.Resource;

namespace Lattice.Shared.DesignSystem.Icons;

public sealed class IconRegistry(IResourceLoader resources)
{
    private const string IconDirectory = "Icons";

    private readonly ConcurrentDictionary<string, Lazy<Task<SvgIconDefinition>>> _cache = new(StringComparer.OrdinalIgnoreCase);

    public async Task<SvgIconDefinition> GetAsync(string name, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        name = name.Trim().ToLowerInvariant();

        if (!IsValidName(name))
        {
            throw new ArgumentException(
                $"Invalid icon name: '{name}'.",
                nameof(name));
        }

        var lazy = _cache.GetOrAdd(
            name,
            key => new Lazy<Task<SvgIconDefinition>>(() => LoadAsync(key), 
                LazyThreadSafetyMode.ExecutionAndPublication));

        try
        {
            // The shared cached load is not cancelled by one caller.
            // Cancellation is checked before awaiting it.
            ct.ThrowIfCancellationRequested();

            return await lazy.Value.WaitAsync(ct);
        }
        catch
        {
            if (lazy.IsValueCreated && lazy.Value.IsFaulted) _cache.TryRemove(name, out _);

            throw;
        }
    }

    private async Task<SvgIconDefinition> LoadAsync(string name)
    {
        var path = $"{IconDirectory}/{name}.svg";

        await using var stream = await resources.OpenAsync(path);

        using var reader = new StreamReader(stream);

        var svg = await reader.ReadToEndAsync();

        var document = XDocument.Parse(svg);

        var root = document.Root ?? throw new InvalidDataException($"SVG resource '{path}' has no root element.");

        if (root.Name.LocalName != "svg")
        {
            throw new InvalidDataException($"Resource '{path}' is not a valid SVG document.");
        }

        var viewBox = (string?)root.Attribute("viewBox")
            ?? "0 0 24 24";

        var content = string.Concat(root.Elements().Select(element =>
            element.ToString(SaveOptions.DisableFormatting)));

        return new SvgIconDefinition(name, viewBox, content);
    }

    private static bool IsValidName(string name)
    {
        return name.Length > 0 && name.All(c =>
                   char.IsAsciiLetterLower(c) ||
                   char.IsAsciiDigit(c) ||
                   c == '-');
    }

    public bool IsLoaded(string name)
    {
        return _cache.TryGetValue(name, out var lazy) &&
               lazy.IsValueCreated &&
               lazy.Value.IsCompletedSuccessfully;
    }

    public int CachedIconCount => _cache.Count;
}