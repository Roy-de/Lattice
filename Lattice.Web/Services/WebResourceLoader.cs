using Lattice.Shared.Resource;

namespace Lattice.Web.Services;

public sealed class WebResourceLoader : IResourceLoader
{
    private readonly HttpClient _http;

    public WebResourceLoader(HttpClient http)
    {
        _http = http;
    }

    public Task<Stream> OpenAsync(string path)
    {
        return _http.GetStreamAsync(path);
    }
}