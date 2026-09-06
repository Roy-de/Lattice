using System.Text;

namespace Lattice.Shared.DesignSystem.Colors;

public class ColorPaletteCssGenerator : IColorPaletteCssGenerator
{
    public string Generate(ColorPalette palette)
    {
        if (palette.Colors == null || palette.Colors.Count == 0)
            return string.Empty;

        var css = new StringBuilder();
        var safeName = SanitizeName(palette.Name);
        
        // Add CSS custom properties with palette prefix
        css.AppendLine($"/* Color Palette: {palette.Name} (ID: {palette.Id}) */");
        css.AppendLine($".palette-{safeName} {{");
        css.AppendLine($"  --palette-name: \"{palette.Name}\";");
        css.AppendLine($"  --palette-id: \"{palette.Id}\";");
        
        foreach (var color in palette.Colors.OrderBy(c => c.Hierarchy))
        {
            var varName = GenerateCssVariableName(color, safeName);
            css.AppendLine($"  {varName}: {color.Value};");
        }
        
        css.AppendLine($"}}");
        
        // Add individual color classes
        foreach (var color in palette.Colors.OrderBy(c => c.Hierarchy))
        {
            var className = GenerateCssClassName(color, safeName);
            var varName = GenerateCssVariableName(color, safeName);
            
            css.AppendLine($".color-{className} {{ color: var({varName}); }}");
            css.AppendLine($".bg-{className} {{ background-color: var({varName}); }}");
            css.AppendLine($".border-{className} {{ border-color: var({varName}); }}");
        }
        
        // Add utility classes for hierarchy-based colors
        foreach (var color in palette.Colors.OrderBy(c => c.Hierarchy))
        {
            var varName = GenerateCssVariableName(color, safeName);
            var hierarchy = color.Hierarchy;
            
            css.AppendLine($".color-hierarchy-{hierarchy} {{ color: var({varName}); }}");
            css.AppendLine($".bg-hierarchy-{hierarchy} {{ background-color: var({varName}); }}");
        }
        
        return css.ToString();
    }

    public string GenerateAll(IEnumerable<ColorPalette> palettes)
    {
        var combinedCss = new StringBuilder();
        combinedCss.AppendLine("/* ===== All Color Palettes ===== */");
        combinedCss.AppendLine(":root {");
        
        // Add all colors from all palettes as global CSS variables with prefixes
        foreach (var palette in palettes)
        {
            var safeName = SanitizeName(palette.Name);
            
            foreach (var color in palette.Colors.OrderBy(c => c.Hierarchy))
            {
                var varName = GenerateCssVariableName(color, safeName);
                combinedCss.AppendLine($"  {varName}: {color.Value};");
            }
        }
        
        combinedCss.AppendLine("}");
        
        // Add individual palette CSS
        foreach (var palette in palettes.OrderBy(p => p.Name))
        {
            combinedCss.AppendLine();
            combinedCss.Append(Generate(palette));
        }
        
        // Add a data attribute selector for switching palettes
        combinedCss.AppendLine();
        combinedCss.AppendLine("/* Palette switching via data attribute */");
        combinedCss.AppendLine("[data-palette] {");
        combinedCss.AppendLine("  /* Define all palette colors as custom properties */");
        combinedCss.AppendLine("}");
        
        return combinedCss.ToString();
    }

    private string SanitizeName(string name)
    {
        return name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }

    private string GenerateCssVariableName(ColorDefinition color, string paletteName)
    {
        var colorName = SanitizeName(color.Name);
        return $"--palette-{paletteName}-{colorName}";
    }

    private string GenerateCssClassName(ColorDefinition color, string paletteName)
    {
        return $"{paletteName}-{SanitizeName(color.Name)}";
    }
}