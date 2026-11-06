namespace Lattice.Shared.DesignSystem.Typography;

public sealed class TypographyScssGeneratorOptions
{
    /// <summary>Lower viewport bound for fluid clamp().</summary>
    public double MinimumViewportWidth { get; set; } = 360;

    /// <summary>Upper viewport bound for fluid clamp().</summary>
    public double MaximumViewportWidth { get; set; } = 1440;

    /// <summary>Emit clamp() between mobile/desktop when they differ.</summary>
    public bool GenerateFluidTypography { get; set; } = true;

    /// <summary>Decimal places in generated CSS numbers.</summary>
    public int DecimalPrecision { get; set; } = 4;

    /// <summary>Attribute selector used to scope the theme, e.g. [data-theme="lattice"]</summary>
    public string ThemeAttribute { get; set; } = "data-theme";

    /// <summary>
    /// Fallback stacks appended after the primary font-family.
    /// Keys are matched case-insensitively against TextStyleDefinition.Family.
    /// </summary>
    public Dictionary<string, string> FontFallbacks { get; set; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Vadodara"] = "system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif",
            ["Geist"]    = "system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif",
            ["Manrope"]  = "system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif"
        };

    /// <summary>
    /// Styles whose names start with these prefixes get tabular numerals
    /// and OpenType numeric features emitted. Case-insensitive.
    /// </summary>
    public string[] NumericStylePrefixes { get; set; } = ["number-"];
}