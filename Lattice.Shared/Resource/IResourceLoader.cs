namespace Lattice.Shared.Resource;

public interface IResourceLoader
{
    /// <summary>
    /// Opens a resource using its logical path.
    /// Example: Resources/Colors/lattice.json
    /// </summary>
    Task<Stream> OpenAsync(string relativePath, CancellationToken ct = default);

    /// <summary>
    /// Lists resources under a directory.
    /// Example: Resources/Icons
    /// </summary>
    Task<IReadOnlyList<string>> ListAsync(string directory, CancellationToken ct = default);

    Task<bool> ExistsAsync(string relativePath, CancellationToken ct = default);
}