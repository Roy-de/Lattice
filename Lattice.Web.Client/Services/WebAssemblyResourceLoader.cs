using Lattice.Shared.Resource;

namespace Lattice.Web.Client.Services;

public class WebAssemblyResourceLoader : IResourceLoader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebAssemblyResourceLoader> _logger;
    private readonly string _basePath;

    public WebAssemblyResourceLoader(
        HttpClient httpClient,
        ILogger<WebAssemblyResourceLoader> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _basePath = ""; // Base path for wwwroot
    }

    public async Task<Stream> OpenAsync(string path)
    {
        try
        {
            var normalizedPath = path.Replace('\\', '/').TrimStart('/');
            var fullPath = $"{_basePath}/{normalizedPath}";
            
            _logger.LogInformation("Attempting to load: {Path}", fullPath);

            var response = await _httpClient.GetAsync(fullPath);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Resource not found: {Path} (Status: {StatusCode})", fullPath, response.StatusCode);
                throw new FileNotFoundException($"Resource not found: {path}");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            _logger.LogInformation("Loaded: {Path} ({Size} bytes)", fullPath, stream.Length);
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
            var normalizedDirectory = directory.Replace('\\', '/').TrimStart('/').TrimEnd('/');
            
            // For WASM, we need a different approach since we can't list files from wwwroot directly
            // We'll try to list files via a manifest or fallback to checking known files
            
            _logger.LogWarning("ListAsync is not fully supported in WASM environment. Returning empty list.");
            return await Task.FromResult(Enumerable.Empty<string>());
            
            // Alternative: If you maintain a manifest.json in wwwroot
            // var manifestPath = $"{_basePath}/manifest.json";
            // var response = await _httpClient.GetAsync(manifestPath);
            // if (response.IsSuccessStatusCode)
            // {
            //     var json = await response.Content.ReadAsStringAsync();
            //     var manifest = JsonSerializer.Deserialize<Manifest>(json);
            //     return manifest.Files
            //         .Where(f => f.StartsWith(normalizedDirectory))
            //         .Select(f => f.Substring(normalizedDirectory.Length + 1))
            //         .ToList();
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list directory: {Directory}", directory);
            return Enumerable.Empty<string>();
        }
    }
}