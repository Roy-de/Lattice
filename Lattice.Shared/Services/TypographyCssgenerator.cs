using System.Text;
using Lattice.Shared.DesignSystem.Typography;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.Services;

public sealed class TypographyCssGenerator : ITypographyCssGenerator
{
    private readonly ILogger<TypographyCssGenerator> _logger;

    public TypographyCssGenerator(ILogger<TypographyCssGenerator> logger)
    {
        _logger = logger;
    }

    public string Generate(TypographyDefinition typography)
    {
        var css = new StringBuilder();

        foreach (var (familyName, font) in typography.Fonts)
        {
            GenerateFontFace(css, familyName, font.Normal);

            if (font.Italic is not null)
            {
                GenerateFontFace(css, familyName, font.Italic, italic: true);
            }
        }

        foreach (var (name, definition) in typography.TextStyles)
        {
            GenerateTextStyle(css, name, definition);
        }

        var generatedCss = css.ToString();

        return generatedCss;
    }

    private static void GenerateFontFace(StringBuilder css, string family, FontStyleDefinition definition, bool italic = false)
    {
        if (definition.Variable is not null)
        {
            var variable = definition.Variable;
            var fontPath = variable.Path.Replace('\\', '/');

            var minWeight = variable.Weight.Min();
            var maxWeight = variable.Weight.Max();

            css.AppendLine("@font-face {");
            css.AppendLine($"    font-family: '{family}';");
            css.AppendLine($"    src: url('/Fonts/{fontPath}') format('truetype');");
            css.AppendLine($"    font-weight: {minWeight} {maxWeight};");
            css.AppendLine($"    font-style: {(italic ? "italic" : "normal")};");
            css.AppendLine("    font-display: swap;");
            css.AppendLine("}");
            css.AppendLine();
        }

        foreach (var (weight, path) in definition.Static)
        {
            css.AppendLine("@font-face {");
            css.AppendLine($"    font-family: '{family}';");
            var fontPath = path.Replace('\\', '/');
            css.AppendLine($"    src: url('/Fonts/{fontPath}') format('truetype');");
            css.AppendLine($"    font-weight: {weight};");
            css.AppendLine($"    font-style: {(italic ? "italic" : "normal")};");
            css.AppendLine("    font-display: swap;");
            css.AppendLine("}");
            css.AppendLine();
        }
    }

    private static void GenerateTextStyle(StringBuilder css, string name, TextStyleDefinition definition)
    {
        css.AppendLine($".text-{name} {{");
        css.AppendLine($"    font-family: '{definition.Family}';");
        css.AppendLine($"    font-weight: {definition.Weight};");
        css.AppendLine($"    font-size: {definition.Desktop.Size};");
        css.AppendLine($"    line-height: {definition.Desktop.LineHeight};");
        css.AppendLine($"    letter-spacing: {definition.Desktop.LetterSpacing};");
        css.AppendLine("}");
        css.AppendLine();

        css.AppendLine("@media (max-width: 768px) {");
        css.AppendLine($"    .text-{name} {{");
        css.AppendLine($"        font-size: {definition.Mobile.Size};");
        css.AppendLine($"        line-height: {definition.Mobile.LineHeight};");
        css.AppendLine($"        letter-spacing: {definition.Mobile.LetterSpacing};");
        css.AppendLine("    }");
        css.AppendLine("}");
        css.AppendLine();
    }
}