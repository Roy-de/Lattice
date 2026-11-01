namespace Lattice.Shared.Resource;

using System.Reflection;

public sealed class EmbeddedUiResourceLoader(HttpClient http, string? remoteBase = null) : IResourceLoader
{
    private readonly string? _remoteBase = string.IsNullOrWhiteSpace(remoteBase) ? null : remoteBase.TrimEnd('/');
    private readonly Assembly _assembly = typeof(EmbeddedUiResourceLoader).Assembly;

    public Task<Stream> OpenAsync(string basePath, string path)
    {
        throw new NotImplementedException();
    }

    public async Task<Stream> OpenAsync(string relativePath, CancellationToken ct = default)
    {
        relativePath = relativePath.Replace('\\', '/').TrimStart('/');

        // 1) Remote first
        if (_remoteBase is not null)
        {
            try
            {
                var remoteUrl = $"{_remoteBase}/{relativePath}";
                var response = await http.GetAsync(remoteUrl, HttpCompletionOption.ResponseHeadersRead, ct);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStreamAsync(ct);
            }
            catch (HttpRequestException) { }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested) { }
        }

        // 2) Embedded fallback
        var resourceName = $"{_assembly.GetName().Name}.wwwroot.Resources."
                           + relativePath.Replace('/', '.');

        var stream = _assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

        return stream;
    }

    public Task<IEnumerable<string>> ListAsync(string directory)
    {
        throw new NotImplementedException();
    }
}