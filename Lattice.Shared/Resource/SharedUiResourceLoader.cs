using System.Net;

namespace Lattice.Shared.Resource;

public class SharedUiResourceLoader(HttpClient http, string? remoteBase = null, string? assemblyName = null)
    : IResourceLoader
{
    private readonly string? _remoteBase = string.IsNullOrWhiteSpace(remoteBase) ? null : remoteBase.TrimEnd('/');
    private readonly string _assemblyName = assemblyName ?? typeof(SharedUiResourceLoader).Assembly.GetName().Name!;

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
            catch (HttpRequestException) { /* fall through */ }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested) { /* timeout, fall through */ }
        }

        // 2) RCL static asset fallback
        var fallbackUrl = $"_content/{_assemblyName}/Resources/{relativePath}";
        var fallback = await http.GetAsync(fallbackUrl, HttpCompletionOption.ResponseHeadersRead, ct);
        if (fallback.StatusCode == HttpStatusCode.NotFound)
            throw new FileNotFoundException($"Resource not found (remote + fallback): {relativePath}");
        fallback.EnsureSuccessStatusCode();
        return await fallback.Content.ReadAsStreamAsync(ct);
    }

    public Task<IEnumerable<string>> ListAsync(string directory)
    {
        throw new NotImplementedException();
    }
}