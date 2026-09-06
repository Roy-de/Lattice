namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorDefinition
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Determines where this token sits in the visual hierarchy.
    /// Lower values appear first.
    /// </summary>
    public int Hierarchy { get; set; }

    /// <summary>
    /// Semantic role of the color.
    /// </summary>
    public ColorRole Role { get; set; }

    /// <summary>
    /// Functional category of the token.
    /// </summary>
    public ColorCategory Category { get; set; }

    /// <summary>
    /// Human-readable description of what this color communicates.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Guidance for designers/developers using this token.
    /// </summary>
    public string Usage { get; set; } = string.Empty;

    /// <summary>
    /// Light theme value.
    /// </summary>
    public string LightValue { get; set; } = string.Empty;

    /// <summary>
    /// Dark theme value.
    /// </summary>
    public string DarkValue { get; set; } = string.Empty;

    /// <summary>
    /// Optional color used for content placed on top of this color.
    /// </summary>
    public string OnLightValue { get; set; } = string.Empty;

    /// <summary>
    /// Optional color used for content placed on top of this color
    /// in dark mode.
    /// </summary>
    public string OnDarkValue { get; set; } = string.Empty;

    /// <summary>
    /// Whether this token is intended to be interactive.
    /// </summary>
    public bool Interactive { get; set; }

    /// <summary>
    /// Whether this token communicates semantic meaning.
    /// </summary>
    public bool Semantic { get; set; }

    /// <summary>
    /// Whether this token is intended primarily for text.
    /// </summary>
    public bool TextColor { get; set; }

    /// <summary>
    /// Whether this token is intended primarily for surfaces.
    /// </summary>
    public bool SurfaceColor { get; set; }

    /// <summary>
    /// Whether this token can safely be used as a UI boundary.
    /// </summary>
    public bool BorderColor { get; set; }

    /// <summary>
    /// Optional WCAG contrast ratio against the primary light background.
    /// </summary>
    public double? LightContrastRatio { get; set; }

    /// <summary>
    /// Optional WCAG contrast ratio against the primary dark background.
    /// </summary>
    public double? DarkContrastRatio { get; set; }

    /// <summary>
    /// Optional related token IDs.
    /// Useful for hover, pressed, disabled, etc.
    /// </summary>
    public List<string> RelatedColors { get; set; } = [];
}