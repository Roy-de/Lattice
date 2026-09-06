namespace Lattice.Shared.Resource;

public interface IResourceLoader
{
    Task<Stream> OpenAsync(string path);
    Task<IEnumerable<string>> ListAsync(string directory);
}