using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPalette
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Personality { get; set; } = string.Empty;

    public List<string> IntendedFor { get; set; } = [];

    public List<string> Keywords { get; set; } = [];

    public ColorModelDefinition? ColorModel { get; set; }

    public JsonElement Primitives { get; set; }

    public JsonElement Semantic { get; set; }

    public JsonElement Components { get; set; }

    public AccessibilityDefinition? Accessibility { get; set; }

    // Palette authoring guidance and provenance vary between sources, so the
    // complete metadata object is preserved instead of being flattened.
    public JsonElement Metadata { get; set; }

    // Palette files can introduce additional metadata without requiring a
    // model release. The full JSON shape remains available to consumers.
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalData { get; set; } = [];
}

public sealed class ColorModelDefinition
{
    public string Space { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public bool SupportsThemes { get; set; }
    public List<string> Themes { get; set; } = [];
}

public sealed class AccessibilityDefinition
{
    public string ContrastStandard { get; set; } = string.Empty;
    public MinimumContrast MinimumContrast { get; set; } = new();
    public Focus Focus { get; set; } = new();
    public ColorIndependence ColorIndependence { get; set; } = new();
    public RecommendedStatusPairing RecommendedStatusPairing { get; set; } = new();
}

public sealed class MinimumContrast
{
    public double NormalTextAA { get; set; }
    public double LargeTextAA { get; set; }
    public double NormalTextAAA { get; set; }
    public double LargeTextAAA { get; set; }
    public double NonTextUI { get; set; }
}

public sealed class Focus
{
    public bool Required { get; set; }
    public double MinimumContrast { get; set; }
    public bool MustNotRelyOnColorAlone { get; set; }
}

public sealed class ColorIndependence
{
    public bool Required { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class RecommendedStatusPairing
{
    public List<string> Success { get; set; } = new();
    public List<string> Warning { get; set; } = new();
    public List<string> Error { get; set; } = new();
    public List<string> Info { get; set; } = new();
}
