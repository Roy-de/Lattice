using System.Reflection;
using System.Text.RegularExpressions;
using Lattice.Shared.Services;

namespace Lattice.Services.IconService;

public class ApplicationIcons : IApplicationIcons
{
    private readonly Dictionary<string, string> _icons = LoadIcons();


    private static Dictionary<string, string> LoadIcons()
    {
        Console.WriteLine("Starting loading icons...");

        var assembly = typeof(ApplicationIcons).Assembly;

        var resources = assembly.GetManifestResourceNames();

        Console.WriteLine($"Assembly: {assembly.FullName}");
        Console.WriteLine($"Resources found: {resources.Length}");

        var svgResources = resources
            .Where(name => name.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine($"SVG resources found: {svgResources.Count}");

        return svgResources.ToDictionary(GetIconName, name => ReadResource(assembly, name), StringComparer.OrdinalIgnoreCase);
    }
    
    private static string GetIconName(string resourceName)
    {
        return Path.GetFileNameWithoutExtension(resourceName);
    }

    private static string ReadResource( Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Unable to load SVG resource '{resourceName}'.");

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    public string GetIcon(string name, int? width = null, int? height = null)
    {
        if (!_icons.TryGetValue(name, out var svg))
        {
            throw new KeyNotFoundException($"Icon '{name}' was not found.");
        }

        return SetSize(svg, width, height);
    }

    public bool Exists(string name)
    {
        return _icons.ContainsKey(name);
    }

    public IReadOnlyCollection<string> GetIconNames()
    {
        return _icons.Keys;
    }

    private static string SetSize(string svg, int? width, int? height)
    {
        if (width is null && height is null)
            return svg;

        if (width is not null)
        {
            svg = Regex.Replace(
                svg,
                @"(<svg\b[^>]*?)\swidth\s*=\s*[""'][^""']*[""']",
                $"$1 width=\"{width}\"",
                RegexOptions.IgnoreCase
            );

            if (!svg.Contains("width=", StringComparison.OrdinalIgnoreCase))
            {
                svg = Regex.Replace(
                    svg,
                    @"<svg\b",
                    $"<svg width=\"{width}\"",
                    RegexOptions.IgnoreCase
                );
            }
        }

        if (height is not null)
        {
            svg = Regex.Replace(
                svg,
                @"(<svg\b[^>]*?)\sheight\s*=\s*[""'][^""']*[""']",
                $"$1 height=\"{height}\"",
                RegexOptions.IgnoreCase
            );

            if (!svg.Contains("height=", StringComparison.OrdinalIgnoreCase))
            {
                svg = Regex.Replace(
                    svg,
                    @"<svg\b",
                    $"<svg height=\"{height}\"",
                    RegexOptions.IgnoreCase
                );
            }
        }

        return svg;
    }
}