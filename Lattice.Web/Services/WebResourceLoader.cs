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
        
        _logger.LogInformation(
            "WebResourceLoader using WebRootPath: {WebRootPath}, FileProvider: {FileProvider}",
            _environment.WebRootPath ?? "<static-assets>",
            _environment.WebRootFileProvider.GetType().Name);
    }

    public Task<Stream> OpenAsync(string basePath, string path)
    {
        try
        {
            // Normalize the path
            var normalizedPath = NormalizePath(basePath, path);
            var fileInfo = _environment.WebRootFileProvider.GetFileInfo(normalizedPath);

            _logger.LogInformation("Attempting to load: {Path}", normalizedPath);

            // Check if file exists
            if (!fileInfo.Exists || fileInfo.IsDirectory)
            {
                throw new FileNotFoundException($"Resource not found: {path}. Normalized path: {normalizedPath}");
            }

            var stream = fileInfo.CreateReadStream();
            _logger.LogInformation("Loaded: {Path} ({Size} bytes)", normalizedPath, fileInfo.Length);
            
            return Task.FromResult<Stream>(stream);
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
            var normalizedDirectory = NormalizeSegment(directory);
            var fullPath = string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? null
                : Path.Combine(
                    _environment.WebRootPath,
                    normalizedDirectory.Replace('/', Path.DirectorySeparatorChar));

            _logger.LogInformation(
                "Attempting to list directory: {Path}",
                normalizedDirectory);

            var providerEntries = _environment.WebRootFileProvider
                .GetDirectoryContents(normalizedDirectory);

            if (providerEntries.Exists)
            {
                var providerFiles = providerEntries
                    .Where(entry => entry.Exists && !entry.IsDirectory)
                    .Select(entry => entry.Name.Replace('\\', '/'))
                    .ToList();

                _logger.LogInformation("Found {Count} files in directory: {Directory}", providerFiles.Count, directory);
                return providerFiles;
            }

            if (string.IsNullOrWhiteSpace(fullPath) || !Directory.Exists(fullPath))
            {
                _logger.LogWarning("Directory not found: {Path}", normalizedDirectory);
                return Enumerable.Empty<string>();
            }

            // Get all files recursively
            var allFiles = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);
            
            // Convert to relative paths
            var relativeFiles = allFiles
                .Select(f => Path.GetRelativePath(_environment.WebRootPath, f).Replace('\\', '/'))
                .ToList();

            _logger.LogInformation("Found {Count} files in directory: {Directory}", relativeFiles.Count, directory);
            
            return relativeFiles;
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

        if (normalized.Split('/').Any(segment => segment is "." or ".."))
            throw new ArgumentException("Resource paths cannot contain '.' or '..' segments.", nameof(value));

        return normalized;
    }
}
