namespace Lattice.Shared.DesignSystem.Semantics;

public sealed class TokenRegistry(TokenInitializer initializer)
{
    public SemanticTokenDocument? GetDocument(string id) =>
        initializer.Documents.TryGetValue(id, out var doc) ? doc : null;

    public SemanticTokenDocument RequireDocument(string id) =>
        GetDocument(id) ?? throw new InvalidOperationException(
            $"Semantic token document '{id}' is not loaded.");

    public IEnumerable<SemanticTokenDocument> AllDocuments =>
        initializer.Documents.Values;
}