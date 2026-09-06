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

    public async Task<Stream> OpenAsync(string basePath, string path)
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
            var prefixedPath = $"{resourcePath}";
            stream = _assembly.GetManifestResourceStream(prefixedPath);
            
            if (stream != null)
            {
                _logger.LogInformation("Loaded resource with prefix: {Path}", prefixedPath);
                return stream;
            }

            // Fallback to file system (for development)
            var filePath = Path.Combine(FileSystem.AppDataDirectory, basePath, Path.GetFileName(path));
            if (File.Exists(filePath))
            {
                _logger.LogInformation("Loaded resource from file system: {Path}", filePath);
                return File.OpenRead(filePath);
            }

            // Last fallback - try from current directory
            var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath, Path.GetFileName(path));
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

    public async Task<IEnumerable<string>> ListAsync(string directory)
    {
        try
        {
            var results = new List<string>();
            
            // Normalize directory path
            directory = directory.TrimStart('/').TrimEnd('/');
            
            // Get all embedded resource names for debugging
            var resourceNames = _assembly.GetManifestResourceNames();
            
            _logger.LogDebug("Total embedded resources: {Count}", resourceNames.Length);
            
            foreach (var resourceName in resourceNames)
            {
                // Check for resources with slash notation (Fonts/file.json)
                if (resourceName.StartsWith($"{directory}/", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = resourceName.Substring($"{directory}/".Length);
                    if (!results.Contains(fileName))
                    {
                        results.Add(fileName);
                        _logger.LogDebug("Found resource (slash notation): {FileName}", fileName);
                    }
                }
                // Check for resources with dot notation (Fonts.file.json)
                else if (resourceName.StartsWith($"{directory}.", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = resourceName.Substring($"{directory}.".Length);
                    if (!results.Contains(fileName))
                    {
                        results.Add(fileName);
                        _logger.LogDebug("Found resource (dot notation): {FileName}", fileName);
                    }
                }
                // Check for resources with Lattice. prefix and dot notation
                else if (resourceName.StartsWith($"Lattice.{directory}.", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = resourceName.Substring($"Lattice.{directory}.".Length);
                    if (!results.Contains(fileName))
                    {
                        results.Add(fileName);
                        _logger.LogDebug("Found resource (Lattice dot notation): {FileName}", fileName);
                    }
                }
                // Check for resources with Lattice. prefix and slash notation
                else if (resourceName.StartsWith($"Lattice.{directory}/", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = resourceName.Substring($"Lattice.{directory}/".Length);
                    if (!results.Contains(fileName))
                    {
                        results.Add(fileName);
                        _logger.LogDebug("Found resource (Lattice slash notation): {FileName}", fileName);
                    }
                }
                // Fallback: check if resource name contains the directory
                else if (resourceName.Contains($"/{directory}/", StringComparison.OrdinalIgnoreCase) ||
                         resourceName.Contains($".{directory}.", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract filename from the end
                    var parts = resourceName.Split('/');
                    var fileName = parts[^1]; // Last part
                    
                    // Also check if it might be dot notation
                    if (!fileName.Contains('/'))
                    {
                        var dotParts = fileName.Split('.');
                        if (dotParts.Length > 1 && dotParts[0] == directory)
                        {
                            fileName = string.Join(".", dotParts.Skip(1));
                        }
                    }
                    
                    if (!results.Contains(fileName))
                    {
                        results.Add(fileName);
                        _logger.LogDebug("Found resource (contains directory): {FileName} from {ResourceName}", fileName, resourceName);
                    }
                }
            }
            
            // Also check file system (for development)
            try
            {
                var basePaths = new[]
                {
                    AppDomain.CurrentDomain.BaseDirectory,
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."),
                    FileSystem.AppDataDirectory
                };
                
                foreach (var basePath in basePaths)
                {
                    if (!string.IsNullOrEmpty(basePath))
                    {
                        var fontsPath = Path.Combine(basePath, directory);
                        if (Directory.Exists(fontsPath))
                        {
                            var files = Directory.GetFiles(fontsPath, "*.json")
                                .Select(Path.GetFileName)
                                .Where(f => f != null && !results.Contains(f));
                            
                            results.AddRange(files!);
                            _logger.LogDebug("Found {Count} files in file system: {Path}", files.Count(), fontsPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Could not list files from file system");
            }
            
            // Remove duplicates and filter to only JSON files
            results = results
                .Distinct()
                .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            _logger.LogInformation("Found {Count} resources in directory '{Directory}': {Files}", 
                results.Count, directory, string.Join(", ", results));
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list resources in directory: {Directory}", directory);
            throw;
        }
    }
}