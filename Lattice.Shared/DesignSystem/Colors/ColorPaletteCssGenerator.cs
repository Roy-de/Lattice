using System.Text;
using System.Text.Json;

namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPaletteCssGenerator : IColorPaletteCssGenerator
{
    public string Generate(ColorPalette palette)
    {
        var tokens = ColorTokenResolver.GetSemanticTokens(palette);
        if (tokens.Count == 0) return string.Empty;
        var css = new StringBuilder($"/* Palette: {palette.Name} ({palette.Id}) */{Environment.NewLine}");
        var safeName = SanitizeName(palette.Name);
        GenerateTheme(css, palette, tokens, safeName, "light");
        GenerateTheme(css, palette, tokens, safeName, "dark");
        return css.ToString();
    }

    private static void GenerateTheme(StringBuilder css, ColorPalette palette, IEnumerable<ColorToken> tokens, string safeName, string theme)
    {
        css.AppendLine().AppendLine($".palette-{safeName}[data-theme=\"{theme}\"] {{");
        foreach (var token in tokens)
        {
            var value = theme == "dark" ? token.DarkValue : token.LightValue;
            if (!string.IsNullOrWhiteSpace(value)) css.AppendLine($"  {ColorTokenResolver.ToCssVariable("color", token.Path)}: {value};");
        }
        foreach (var (path, reference) in EnumerateComponentReferences(palette.Components))
        {
            var value = ToCssValue(palette, reference, theme);
            if (!string.IsNullOrWhiteSpace(value)) css.AppendLine($"  {ColorTokenResolver.ToCssVariable("component", path)}: {value};");
        }
        css.AppendLine("}");
    }

    private static IEnumerable<(string Path, string Reference)> EnumerateComponentReferences(JsonElement node, string path = "")
    {
        if (node.ValueKind == JsonValueKind.String) { yield return (path, node.GetString() ?? string.Empty); yield break; }
        if (node.ValueKind != JsonValueKind.Object) yield break;
        foreach (var property in node.EnumerateObject())
            foreach (var item in EnumerateComponentReferences(property.Value, string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}")) yield return item;
    }

    private static string ToCssValue(ColorPalette palette, string reference, string theme)
    {
        var resolved = ColorTokenResolver.Resolve(palette, reference, theme);
        if (!string.IsNullOrWhiteSpace(resolved)) return resolved;
        reference = reference.Trim().Trim('{', '}');
        return reference.Contains('.') ? $"var({ColorTokenResolver.ToCssVariable("color", reference.Replace("semantic.", "", StringComparison.OrdinalIgnoreCase))})" : reference;
    }

    private static string SanitizeName(string value) => string.Concat(value.Trim().ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) ? c : '-')).Trim('-');
    public string GenerateAll(IEnumerable<ColorPalette> palettes) => string.Concat(palettes.OrderBy(p => p.Name).Select(Generate));
}
