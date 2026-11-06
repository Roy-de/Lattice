namespace Lattice.Shared.DesignSystem.Semantics;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

// ============================================================
// Root
// ============================================================
public sealed class SemanticTokenDocument
{
    [JsonPropertyName("$schema")] public string? Schema { get; set; }

    [JsonPropertyName("metadata")] public Metadata Metadata { get; set; } = new();

    [JsonPropertyName("policies")] public Dictionary<string, PolicyDefinition> Policies { get; set; } = new();

    [JsonPropertyName("categories")] public List<Category> Categories { get; set; } = new();

    [JsonPropertyName("tokens")] public List<Token> Tokens { get; set; } = new();

    [JsonPropertyName("themes")] public Dictionary<string, Theme> Themes { get; set; } = new();

    [JsonPropertyName("components")] public List<Component> Components { get; set; } = new();

    [JsonPropertyName("validation")] public ValidationRules Validation { get; set; } = new();
}

// ============================================================
// Metadata
// ============================================================
public sealed class Metadata
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("schemaVersion")] public string SchemaVersion { get; set; } = string.Empty;

    [JsonPropertyName("tokenVersion")] public string TokenVersion { get; set; } = string.Empty;

    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    [JsonPropertyName("defaultTheme")] public string DefaultTheme { get; set; } = string.Empty;

    [JsonPropertyName("supportedThemes")] public List<string> SupportedThemes { get; set; } = new();

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    [JsonPropertyName("primitiveReferenceFormat")]
    public string PrimitiveReferenceFormat { get; set; } = string.Empty;

    [JsonPropertyName("notes")] public List<string> Notes { get; set; } = new();
}

// ============================================================
// Policies
// ============================================================
public sealed class PolicyDefinition
{
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    [JsonPropertyName("themeOverridable")] public bool ThemeOverridable { get; set; }

    [JsonPropertyName("userOverridable")] public bool UserOverridable { get; set; }
}

// ============================================================
// Categories
// ============================================================
public sealed class Category
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    [JsonPropertyName("order")] public int Order { get; set; }
}

// ============================================================
// Token
// ============================================================
public sealed class Token
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("category")] public string Category { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

    [JsonPropertyName("customization")] public Customization Customization { get; set; } = new();

    [JsonPropertyName("constraints")] public TokenConstraints Constraints { get; set; } = new();

    [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Default value(s). Depending on token type this may contain
    /// "light"/"dark" (color), "value" (dimension/duration/easing/shadow),
    /// or "reference" (typography style).
    /// </summary>
    [JsonPropertyName("default")]
    public Dictionary<string, JsonElement> Default { get; set; } = new();
}

public sealed class Customization
{
    [JsonPropertyName("policy")] public string Policy { get; set; } = string.Empty;

    [JsonPropertyName("userOverridable")] public bool UserOverridable { get; set; }
}

public sealed class TokenConstraints
{
    [JsonPropertyName("required")] public bool Required { get; set; }

    [JsonPropertyName("allowAlpha")] public bool? AllowAlpha { get; set; }

    [JsonPropertyName("requiresContrastValidation")]
    public bool? RequiresContrastValidation { get; set; }

    [JsonPropertyName("format")] public string? Format { get; set; }

    [JsonPropertyName("unit")] public string? Unit { get; set; }

    [JsonPropertyName("reducedMotionFallback")]
    public string? ReducedMotionFallback { get; set; }

    [JsonPropertyName("referenceNamespace")]
    public string? ReferenceNamespace { get; set; }
}

// ============================================================
// Themes
// ============================================================
public sealed class Theme
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mode")] public string Mode { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    [JsonPropertyName("mappings")] public Dictionary<string, string> Mappings { get; set; } = new();
}

// ============================================================
// Components
// ============================================================
public sealed class Component
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    [JsonPropertyName("variants")] public List<string> Variants { get; set; } = new();

    [JsonPropertyName("states")] public List<string> States { get; set; } = new();

    [JsonPropertyName("tokenBindings")] public Dictionary<string, string> TokenBindings { get; set; } = new();
}

// ============================================================
// Validation
// ============================================================
public sealed class ValidationRules
{
    [JsonPropertyName("unknownTokenPolicy")]
    public string UnknownTokenPolicy { get; set; } = string.Empty;

    [JsonPropertyName("missingRequiredTokenPolicy")]
    public string MissingRequiredTokenPolicy { get; set; } = string.Empty;

    [JsonPropertyName("invalidReferencePolicy")]
    public string InvalidReferencePolicy { get; set; } = string.Empty;

    [JsonPropertyName("resolutionOrder")] public List<string> ResolutionOrder { get; set; } = new();

    [JsonPropertyName("checks")] public List<ValidationCheck> Checks { get; set; } = new();

    [JsonPropertyName("accessibility")] public AccessibilityRules Accessibility { get; set; } = new();
}

public sealed class ValidationCheck
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
}

public sealed class AccessibilityRules
{
    [JsonPropertyName("standard")] public string Standard { get; set; } = string.Empty;

    [JsonPropertyName("normalTextContrastMinimum")]
    public double NormalTextContrastMinimum { get; set; }

    [JsonPropertyName("largeTextContrastMinimum")]
    public double LargeTextContrastMinimum { get; set; }

    [JsonPropertyName("nonTextContrastMinimum")]
    public double NonTextContrastMinimum { get; set; }

    [JsonPropertyName("note")] public string Note { get; set; } = string.Empty;
}