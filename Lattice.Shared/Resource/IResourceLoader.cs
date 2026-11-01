namespace Lattice.Shared.Resource;

public interface IResourceLoader
{
    Task<Stream> OpenAsync(string basePath, string path);
    
    /// <summary>
    /// Opens a JSON resource by relative path, e.g. "Colors/lattice.json".
    /// Implementations must try remote first (if configured) and fall back
    /// to the RCL default.
    /// </summary>
    Task<Stream> OpenAsync(string relativePath, CancellationToken ct = default);
    Task<IEnumerable<string>> ListAsync(string directory);
}