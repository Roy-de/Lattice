using System.Reflection;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Services;

public class MauiResourceLoader : IResourceLoader
{
    private readonly Assembly _assembly;
    private readonly ILogger<MauiResourceLoader> _logger;

    public MauiResourceLoader(ILogger<MauiResourceLoader> logger)
    {
        _assembly = typeof(MauiResourceLoader).Assembly;
        _logger = logger;
    }

    public async Task<Stream> OpenAsync(string path)
    {
        try
        {
            // Try embedded resource first (for fonts embedded with MauiAsset)
            var resourcePath = path.Replace('/', '.');
            var stream = _assembly.GetManifestResourceStream(resourcePath);
            
            if (stream != null)
            {
                _logger.LogInformation("Loaded resource as embedded: {Path}", path);
                return stream;
            }

            // Try with Lattice. prefix (for resources in the Lattice project)
            var prefixedPath = $"Lattice.{resourcePath}";
            stream = _assembly.GetManifestResourceStream(prefixedPath);
            
            if (stream != null)
            {
                _logger.LogInformation("Loaded resource with prefix: {Path}", prefixedPath);
                return stream;
            }

            // Fallback to file system (for development)
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "Fonts", Path.GetFileName(path));
            if (File.Exists(filePath))
            {
                _logger.LogInformation("Loaded resource from file system: {Path}", filePath);
                return File.OpenRead(filePath);
            }

            // Last fallback - try from current directory
            var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", Path.GetFileName(path));
            if (File.Exists(localPath))
            {
                _logger.LogInformation("Loaded resource from local path: {Path}", localPath);
                return File.OpenRead(localPath);
            }

            throw new FileNotFoundException($"Resource not found: {path}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load resource: {Path}", path);
            throw;
        }
    }
}