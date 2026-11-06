using System.Text.Json;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Semantics;

public sealed class TokenInitializer(IResourceLoader resources,
    ILogger<TokenInitializer>? logger = null)
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly Dictionary<string, SemanticTokenDocument> _documents =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, SemanticTokenDocument> Documents => _documents;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        try
        {
            // Convention: token documents live under "Semantics/" in the RCL
            var files = await resources.ListAsync("Semantics", ct);

            var tokenFiles = files
                .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .ToList();

            logger?.LogInformation(
                "Found {Count} semantic token files",
                tokenFiles.Count);

            foreach (var file in tokenFiles)
            {
                ct.ThrowIfCancellationRequested();

                await using var stream = await resources.OpenAsync(file, ct);

                var doc = await JsonSerializer.DeserializeAsync<SemanticTokenDocument>(
                    stream, _options, ct);

                if (doc is null)
                    throw new InvalidOperationException(
                        $"Failed to deserialize token document '{file}'.");

                if (string.IsNullOrWhiteSpace(doc.Metadata.Id))
                    throw new InvalidOperationException(
                        $"Token document '{file}' is missing metadata.id.");

                _documents[doc.Metadata.Id] = doc;

                logger?.LogInformation(
                    "Loaded token document '{Id}' v{Version} ({TokenCount} tokens)",
                    doc.Metadata.Id,
                    doc.Metadata.TokenVersion,
                    doc.Tokens.Count);
            }

            logger?.LogInformation(
                "Semantic token initialization completed. Loaded {Count} documents",
                _documents.Count);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to initialize semantic token system");
            throw;
        }
    }
}