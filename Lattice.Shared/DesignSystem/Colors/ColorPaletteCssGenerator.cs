using System.Text;

namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPaletteCssGenerator : IColorPaletteCssGenerator
{
    public string Generate(ColorPalette palette)
    {
        if (palette.Colors == null || palette.Colors.Count == 0)
            return string.Empty;

        var css = new StringBuilder();

        var safeName = SanitizeName(palette.Name);

        css.AppendLine(
            $"/* ========================================================= */");
        css.AppendLine(
            $"/* Palette: {palette.Name} ({palette.Id}) */");
        css.AppendLine(
            $"/* {palette.Description} */");
        css.AppendLine(
            $"/* ========================================================= */");

        GenerateTheme(css, palette, safeName, dark: false);
        GenerateTheme(css, palette, safeName, dark: true);

        GenerateColorUtilityClasses(css, palette, safeName);

        return css.ToString();
    }

    private void GenerateTheme(
        StringBuilder css,
        ColorPalette palette,
        string safeName,
        bool dark)
    {
        var selector = dark
            ? $".palette-{safeName}[data-theme=\"dark\"]"
            : $".palette-{safeName}[data-theme=\"light\"]";

        css.AppendLine();
        css.AppendLine($"{selector} {{");

        foreach (var color in palette.Colors
                     .OrderBy(c => c.Hierarchy))
        {
            var value = dark
                ? color.DarkValue
                : color.LightValue;

            if (string.IsNullOrWhiteSpace(value))
                continue;

            var variable = GenerateSemanticVariable(color);

            css.AppendLine(
                $"    {variable}: {value};");

            var onValue = dark
                ? color.OnDarkValue
                : color.OnLightValue;

            if (!string.IsNullOrWhiteSpace(onValue))
            {
                css.AppendLine(
                    $"    {GenerateOnVariable(color)}: {onValue};");
            }
        }

        css.AppendLine("}");
    }

    private void GenerateColorUtilityClasses(
        StringBuilder css,
        ColorPalette palette,
        string safeName)
    {
        foreach (var color in palette.Colors
                     .OrderBy(c => c.Hierarchy))
        {
            var variable = GenerateSemanticVariable(color);

            var className = SanitizeName(color.Id);

            css.AppendLine();
            css.AppendLine(
                $".color-{className} {{");
            css.AppendLine(
                $"    color: var({variable});");
            css.AppendLine("}");

            css.AppendLine(
                $".bg-{className} {{");
            css.AppendLine(
                $"    background-color: var({variable});");
            css.AppendLine("}");

            css.AppendLine(
                $".border-{className} {{");
            css.AppendLine(
                $"    border-color: var({variable});");
            css.AppendLine("}");
        }
    }

    private string GenerateSemanticVariable(
        ColorDefinition color)
    {
        return $"--color-{ToKebabCase(color.Role.ToString())}";
    }

    private string GenerateOnVariable(
        ColorDefinition color)
    {
        return $"--color-on-{ToKebabCase(color.Role.ToString())}";
    }

    private static string ToKebabCase(string value)
    {
        var result = new StringBuilder();

        for (var i = 0; i < value.Length; i++)
        {
            var character = value[i];

            if (char.IsUpper(character) && i > 0)
                result.Append('-');

            result.Append(char.ToLowerInvariant(character));
        }

        return result.ToString();
    }

    private static string SanitizeName(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }

    public string GenerateAll(
        IEnumerable<ColorPalette> palettes)
    {
        var css = new StringBuilder();

        css.AppendLine(
            "/* ========================================================= */");
        css.AppendLine(
            "/* Lattice Design System - Color Palettes */");
        css.AppendLine(
            "/* ========================================================= */");

        foreach (var palette in palettes.OrderBy(p => p.Name))
        {
            css.AppendLine();
            css.Append(Generate(palette));
        }

        return css.ToString();
    }
}