using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.Resource;

public sealed class EmbeddedResourceLoader : IResourceLoader
{
    private const string ResourcePrefix = "Lattice.Shared.Resource/";

    private readonly Assembly _assembly;
    private readonly ILogger<EmbeddedResourceLoader>? _logger;

    // Logical resource path -> actual assembly resource name
    private readonly Dictionary<string, string> _resources;

    public EmbeddedResourceLoader(
        ILogger<EmbeddedResourceLoader>? logger = null)
        : this(typeof(EmbeddedResourceLoader).Assembly, logger)
    {
    }

    public EmbeddedResourceLoader(
        Assembly assembly,
        ILogger<EmbeddedResourceLoader>? logger = null)
    {
        _assembly = assembly;
        _logger = logger;

        _logger?.LogInformation(
            "Starting embedded resource scan. Assembly: {Assembly}, Location: {Location}",
            _assembly.FullName,
            _assembly.Location);

        _logger?.LogInformation("Expected resource prefix: {ResourcePrefix}", ResourcePrefix);

        var manifestResources = _assembly.GetManifestResourceNames();
        
        _resources = manifestResources
            .Where(name => name.StartsWith(
                ResourcePrefix,
                StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                name => name[ResourcePrefix.Length..]
                    .Replace('\\', '/'),
                name => name,
                StringComparer.OrdinalIgnoreCase);

        _logger?.LogInformation("Resource scan completed. Matched {Count} resources", _resources.Count);

    }

    public Task<Stream> OpenAsync(
        string relativePath,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var path = NormalizePath(relativePath);

        if (!_resources.TryGetValue(path, out var resourceName))
        {
            _logger?.LogWarning(
                "Resource not found. Requested path: {Path}",
                path);

            throw new FileNotFoundException(
                $"Resource '{relativePath}' was not found.");
        }

        var stream = _assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new FileNotFoundException(
                $"Unable to open embedded resource '{relativePath}'.");
        }

        _logger?.LogDebug(
            "Opened embedded resource: {ResourceName}",
            resourceName);

        return Task.FromResult(stream);
    }

    public Task<IReadOnlyList<string>> ListAsync(
        string directory,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var normalizedDirectory = NormalizeDirectory(directory);

        var prefix = string.IsNullOrEmpty(normalizedDirectory)
            ? string.Empty
            : normalizedDirectory + "/";

        _logger?.LogInformation(
            "Listing resources. Requested directory: '{Directory}', normalized directory: '{NormalizedDirectory}', search prefix: '{Prefix}'",
            directory,
            normalizedDirectory,
            prefix);

        IReadOnlyList<string> results = _resources.Keys
            .Where(path => path.StartsWith(
                prefix,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _logger?.LogInformation(
            "Found {Count} resources under '{Directory}'",
            results.Count,
            normalizedDirectory);

        foreach (var result in results)
        {
            _logger?.LogDebug(
                "Listed resource: {Resource}",
                result);
        }

        return Task.FromResult(results);
    }

    public Task<bool> ExistsAsync(
        string relativePath,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var path = NormalizePath(relativePath);
        var exists = _resources.ContainsKey(path);

        _logger?.LogDebug(
            "Resource existence check: {Path} = {Exists}",
            path,
            exists);

        return Task.FromResult(exists);
    }

    private static string NormalizePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var normalized = path
            .Replace('\\', '/')
            .Trim('/');

        if (normalized.Split('/').Any(segment =>
                segment is "." or ".."))
        {
            throw new ArgumentException(
                "Resource paths cannot contain traversal segments.",
                nameof(path));
        }

        return normalized;
    }

    private static string NormalizeDirectory(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            return string.Empty;

        return NormalizePath(directory);
    }
}