using Lattice.Shared.Resource;

using System.Net;
using System.Net.Http.Json;

namespace Lattice.Web.Client.Services;

public class WebAssemblyResourceLoader : IResourceLoader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebAssemblyResourceLoader> _logger;
    private const string ManifestPath = "resource-manifest.json";

    public WebAssemblyResourceLoader(
        HttpClient httpClient,
        ILogger<WebAssemblyResourceLoader> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Stream> OpenAsync(string basePath, string path)
    {
        try
        {
            var fullPath = NormalizePath(basePath, path);
            
            _logger.LogInformation("Attempting to load: {Path}", fullPath);

            var response = await _httpClient.GetAsync(fullPath);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Resource not found: {Path} (Status: {StatusCode})", fullPath, response.StatusCode);
                throw new FileNotFoundException($"Resource not found: {path}");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            _logger.LogInformation("Loaded: {Path}", fullPath);
            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load: {Path}", path);
            throw;
        }
    }

    public async Task<IEnumerable<string>> ListAsync(string directory)
    {
        try
        {
            var normalizedDirectory = NormalizeSegment(directory);
            var response = await _httpClient.GetAsync(ManifestPath);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<string>();

            response.EnsureSuccessStatusCode();
            var manifest = await response.Content.ReadFromJsonAsync<ResourceManifest>();

            return manifest?.Files
                       .Where(file => Path.GetDirectoryName(file)?.Replace('\\', '/')
                           .Equals(normalizedDirectory, StringComparison.OrdinalIgnoreCase) == true)
                       .Select(file => Path.GetFileName(file))
                       .Where(file => file.Length > 0)
                       .ToList()
                   ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list directory: {Directory}", directory);
            return Enumerable.Empty<string>();
        }
    }

    private static string NormalizePath(string basePath, string path)
    {
        var normalizedBase = NormalizeSegment(basePath);
        var normalizedPath = NormalizeSegment(path);

        if (string.IsNullOrEmpty(normalizedBase) ||
            normalizedPath.Equals(normalizedBase, StringComparison.OrdinalIgnoreCase) ||
            normalizedPath.StartsWith(normalizedBase + "/", StringComparison.OrdinalIgnoreCase))
        {
            return normalizedPath;
        }

        return $"{normalizedBase}/{normalizedPath}";
    }

    private static string NormalizeSegment(string value)
    {
        var normalized = value.Replace('\\', '/').Trim('/');

        return normalized.Split('/').Any(segment => segment is "." or "..") ? throw new ArgumentException("Resource paths cannot contain '.' or '..' segments.", nameof(value)) : normalized;
    }

    private sealed class ResourceManifest
    {
        public List<string> Files { get; init; } = [];
    }
}
