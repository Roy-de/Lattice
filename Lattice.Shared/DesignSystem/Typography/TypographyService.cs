using System.Text.Json;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Typography;

public sealed class TypographyService : ITypographyService
{
    private readonly IResourceLoader _resources;
    private readonly ILogger<TypographyService> _logger;
    private string? _generatedCss;
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly List<TypographyDefinition> _definitions = new();
    
    public TypographyService(IResourceLoader resources, ILogger<TypographyService> logger)
    {
        _resources = resources;
        _logger = logger;
    }

    public TypographyDefinition Current { get; private set; } = new();
    public IReadOnlyList<TypographyDefinition> Definitions => _definitions.AsReadOnly();

    public async Task LoadAsync(string preset)
    {
        try
        {
            var path = $"Fonts/{preset}.json";
            _logger.LogInformation("Loading typography from {Path}", path);
        
            await using var stream = await _resources.OpenAsync("Fonts", path);
        
            // Read the JSON as string first to log it
            using var reader = new StreamReader(stream);
            var jsonString = await reader.ReadToEndAsync();
            _logger.LogInformation("JSON content (first 200 chars): {Json}", 
                jsonString.Length > 200 ? jsonString.Substring(0, 200) + "..." : jsonString);
        
            // Reset stream position if possible
            stream.Seek(0, SeekOrigin.Begin);
        
            var definition = await JsonSerializer.DeserializeAsync<TypographyDefinition>(stream, _options);
        
            if (definition == null)
                throw new InvalidOperationException($"Failed to deserialize typography preset '{preset}'");
        
            _logger.LogInformation("Deserialized - Fonts count: {Fonts}, Styles count: {Styles}", 
                definition.Fonts?.Count ?? 0, 
                definition.TextStyles?.Count ?? 0);
        
            // Log font keys
            if (definition.Fonts != null)
            {
                _logger.LogInformation("Font keys: {Keys}", string.Join(", ", definition.Fonts.Keys));
            }
        
            // Set as current and add to definitions list
            Current = definition;
            
            // Check if definition with same name already exists
            var existingIndex = _definitions.FindIndex(d => d.Name == definition.Name);
            if (existingIndex >= 0)
            {
                _definitions[existingIndex] = definition;
                _logger.LogInformation("Updated existing typography definition '{Name}'", definition.Name);
            }
            else
            {
                _definitions.Add(definition);
                _logger.LogInformation("Added new typography definition '{Name}'", definition.Name);
            }
            
            _generatedCss = null;
        
            _logger.LogInformation("Loaded typography preset '{Preset}' with {Styles} styles. Total definitions: {Total}", 
                preset, definition.TextStyles?.Count ?? 0, _definitions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load typography preset '{Preset}'", preset);
            throw;
        }
    }

    public TextStyleDefinition GetStyle(string name)
    {
        if (!Current.TextStyles.TryGetValue(name, out var style))
        {
            throw new KeyNotFoundException(
                $"Typography style '{name}' was not found.");
        }

        return style;
    }

    public FontDefinition GetFont(string family)
    {
        if (!Current.Fonts.TryGetValue(family, out var font))
        {
            throw new KeyNotFoundException(
                $"Font family '{family}' was not found.");
        }

        return font;
    }
    
    public string GetGeneratedCss(ITypographyCssGenerator generator)
    {
        if (_generatedCss == null)
        {
            _generatedCss = generator.Generate(Current);
        }
        return _generatedCss;
    }
}