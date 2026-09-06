using System.Reflection;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Web.Services;

public sealed class WebResourceLoader : IResourceLoader
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<WebResourceLoader> _logger;

    public WebResourceLoader(
        IWebHostEnvironment environment,
        ILogger<WebResourceLoader> logger)
    {
        _environment = environment;
        _logger = logger;
        
        _logger.LogInformation("WebResourceLoader using WebRootPath: {WebRootPath}", _environment.WebRootPath);
    }

    public async Task<Stream> OpenAsync(string path)
    {
        try
        {
            // Normalize the path
            var normalizedPath = path.Replace('\\', '/').TrimStart('/');
            var fullPath = Path.Combine(_environment.WebRootPath, normalizedPath);
            
            _logger.LogInformation("Attempting to load: {Path}", fullPath);

            // Check if file exists
            if (!File.Exists(fullPath))
            {
                // Log all font files in wwwroot for debugging
                if (path.Contains("Font", StringComparison.OrdinalIgnoreCase) || 
                    path.EndsWith(".ttf") || 
                    path.EndsWith(".json"))
                {
                    var fontDirectory = Path.Combine(_environment.WebRootPath, "Fonts");
                    if (Directory.Exists(fontDirectory))
                    {
                        var allFontFiles = Directory.GetFiles(fontDirectory, "*.*", SearchOption.AllDirectories);
                        _logger.LogWarning("Available font files in wwwroot/Fonts ({Count}):", allFontFiles.Length);
                        foreach (var file in allFontFiles.Take(20))
                        {
                            var relativePath = Path.GetRelativePath(_environment.WebRootPath, file).Replace('\\', '/');
                            _logger.LogWarning("  - {File}", relativePath);
                        }
                    }
                }

                throw new FileNotFoundException($"Resource not found: {path}. Full path: {fullPath}");
            }

            // Read the file as a stream
            var stream = File.OpenRead(fullPath);
            var fileInfo = new FileInfo(fullPath);
            _logger.LogInformation("Loaded: {Path} ({Size} bytes)", fullPath, fileInfo.Length);
            
            return await Task.FromResult(stream);
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
            // Normalize the directory path
            var normalizedDirectory = directory.Replace('\\', '/').TrimStart('/').TrimEnd('/');
            var fullPath = Path.Combine(_environment.WebRootPath, normalizedDirectory);
            
            _logger.LogInformation("Attempting to list directory: {Path}", fullPath);

            if (!Directory.Exists(fullPath))
            {
                _logger.LogWarning("Directory not found: {Path}", fullPath);
                return Enumerable.Empty<string>();
            }

            // Get all files recursively
            var allFiles = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);
            
            // Convert to relative paths
            var relativeFiles = allFiles
                .Select(f => Path.GetRelativePath(_environment.WebRootPath, f).Replace('\\', '/'))
                .ToList();

            _logger.LogInformation("Found {Count} files in directory: {Directory}", relativeFiles.Count, directory);
            
            return await Task.FromResult(relativeFiles.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list directory: {Directory}", directory);
            return Enumerable.Empty<string>();
        }
    }
}