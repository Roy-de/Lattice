using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPaletteCssGenerator : IColorPaletteCssGenerator
{
    private readonly ILogger<ColorPaletteCssGenerator> _logger;

    public ColorPaletteCssGenerator(ILogger<ColorPaletteCssGenerator> logger)
    {
        _logger = logger;
    }

    public string Generate(ColorPalette palette)
    {
        var tokens = ColorTokenResolver.GetSemanticTokens(palette);
        if (tokens.Count == 0) return string.Empty;
        var css = new StringBuilder($"/* Palette: {palette.Name} ({palette.Id}) */{Environment.NewLine}");
        var safeName = SanitizeName(palette.Name);
        GenerateTheme(css, palette, tokens, safeName, "light");
        GenerateTheme(css, palette, tokens, safeName, "dark");
        _logger.LogInformation("Generated CSS for palette {PaletteId}: {Css}", palette.Id, css.ToString());

        return css.ToString();
    }

    private static void GenerateTheme(StringBuilder css, ColorPalette palette, IEnumerable<ColorToken> tokens,
        string safeName, string theme)
    {
        css.AppendLine().AppendLine($".palette-{safeName}[data-theme=\"{theme}\"] {{");
        foreach (var token in tokens)
        {
            var value = theme == "dark" ? token.DarkValue : token.LightValue;
            if (!string.IsNullOrWhiteSpace(value))
                css.AppendLine($"  {ColorTokenResolver.ToCssVariable("color", token.Path)}: {value};");
        }

        foreach (var (path, reference) in EnumerateComponentReferences(palette.Components))
        {
            var value = ToCssValue(reference);
            if (!string.IsNullOrWhiteSpace(value))
                css.AppendLine($"  {ColorTokenResolver.ToCssVariable("component", path)}: {value};");
        }

        css.AppendLine("}");
    }

    private static IEnumerable<(string Path, string Reference)> EnumerateComponentReferences(JsonElement node,
        string path = "")
    {
        if (node.ValueKind == JsonValueKind.String)
        {
            yield return (path, node.GetString() ?? string.Empty);
            yield break;
        }

        if (node.ValueKind != JsonValueKind.Object) yield break;
        foreach (var property in node.EnumerateObject())
        foreach (var item in EnumerateComponentReferences(property.Value,
                     string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}"))
            yield return item;
    }

    private static string ToCssValue(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
            return string.Empty;

        reference = reference
            .Trim()
            .Trim('{', '}');
        if (IsCssValue(reference))
            return reference;
        if (reference.StartsWith("semantic.", StringComparison.OrdinalIgnoreCase))
        {
            reference = reference["semantic.".Length..];
        }

        if (reference.Contains('.'))
        {
            return $"var({ColorTokenResolver.ToCssVariable("color", reference)})";
        }

        return reference;
    }
    
    private static bool IsCssValue(string value)
    {
        return
            value.StartsWith("#", StringComparison.Ordinal) ||

            value.StartsWith( "rgb", StringComparison.OrdinalIgnoreCase) ||

            value.StartsWith("rgba", StringComparison.OrdinalIgnoreCase) ||

            value.StartsWith("hsl", StringComparison.OrdinalIgnoreCase) ||

            value.StartsWith("hsla", StringComparison.OrdinalIgnoreCase) ||

            value.StartsWith("oklch", StringComparison.OrdinalIgnoreCase) ||

            value.StartsWith("oklab", StringComparison.OrdinalIgnoreCase) ||

            value.StartsWith("color(", StringComparison.OrdinalIgnoreCase) ||

            value.StartsWith("var(", StringComparison.OrdinalIgnoreCase) ||

            value.Equals("transparent", StringComparison.OrdinalIgnoreCase) ||

            value.Equals("currentColor", StringComparison.OrdinalIgnoreCase);
    }

    private static string SanitizeName(string value) => string
        .Concat(value.Trim().ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) ? c : '-')).Trim('-');

    public string GenerateAll(IEnumerable<ColorPalette> palettes) =>
        string.Concat(palettes.OrderBy(p => p.Name).Select(Generate));
}