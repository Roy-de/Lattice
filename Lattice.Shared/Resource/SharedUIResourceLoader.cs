namespace Lattice.Shared.Resource;

public class SharedUiResourceLoader: IResourceLoader
{
    private readonly HttpClient _http;
    private readonly string _remoteBase;

    public SharedUiResourceLoader(HttpClient http, string? remoteBase = null)
    {
        _http = http;
        _remoteBase = remoteBase;
    }
    public Task<Stream> OpenAsync(string basePath, string path)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> ListAsync(string directory)
    {
        throw new NotImplementedException();
    }
}