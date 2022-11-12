using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace fourtynine.Development;

public class ViteManifestException : Exception
{
    public string ManifestPath { get; }
    
    public ViteManifestException(string manifestPath, string message)
        : this(manifestPath, message, null)
    {
    }

    public ViteManifestException(string manifestPath, string? message, Exception? innerException)
        : base($"Manifest {manifestPath}: {message}", innerException)
    {
        ManifestPath = manifestPath;
    }
}

public class ViteManifestService : IViteManifestService
{
    public IReadOnlyDictionary<string, string> FileNameMappings { get; }
    
    /// <summary>
    /// </summary>
    /// <param name="manifestPath"></param>
    /// <exception cref="ViteManifestException">Any problems related to parsing or interpreting the manifest file.</exception>
    public ViteManifestService(string manifestPath)
    {
        if (!File.Exists(manifestPath))
            throw new ViteManifestException(manifestPath, "Manifest file not found");
        
        var manifest = File.ReadAllText(manifestPath);
        
        JsonDocument manifestJson;
        try
        {
            manifestJson = JsonDocument.Parse(manifest);
        }
        catch (JsonException  e)
        {
            throw new ViteManifestException(manifestPath, "Manifest file is not valid JSON", e);
        }

        using (manifestJson)
        {
            FileNameMappings = manifestJson.RootElement
                .EnumerateObject()
                .ToDictionary(
                    prop => prop.Name,
                    prop =>
                    {
                        bool hasProp = prop.Value.TryGetProperty("file", out var filePropValue);
                        if (!hasProp)
                        {
                            var error = $"No property 'file' for entry {prop.Name}";
                            throw new ViteManifestException(manifestPath, error);
                        }

                        if (filePropValue.ValueKind != JsonValueKind.String)
                        {
                            var error = $"Property 'file' for entry {prop.Name} must be a string"
                                + $"but it's in fact '{filePropValue.ValueKind}'";
                            throw new ViteManifestException(manifestPath, error);
                        }
                        
                        return filePropValue.GetString()!;
                    });
        }
    }
}

/// <summary>
/// Used in development, because it only makes sense to look at the manifest
/// when mapping file paths in production. In development, the files are served
/// by Vite directly, so no remapping is ever needed.
/// </summary>
public class ViteManifestIdentityMappingService : IViteManifestService
{
    public IReadOnlyDictionary<string, string> FileNameMappings => IdentityMappings.Instance;

    private class IdentityMappings : IReadOnlyDictionary<string, string>
    {
        public static readonly IdentityMappings Instance = new();
        public bool ContainsKey(string key) => true;
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            value = key;
            return true;
        }
        public string this[string key] => key;
        
        // Enumeration and similar operations are not supported.
        public int Count => throw new NotSupportedException();
        public IEnumerable<string> Keys => throw new NotSupportedException();
        public IEnumerable<string> Values => throw new NotSupportedException();
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => throw new NotSupportedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }
}

public interface IViteManifestService
{
    IReadOnlyDictionary<string, string> FileNameMappings { get; }
}

public static class ViteManifestHelper
{
    public static string RemapFileName(this IViteManifestService manifestService, string fileName)
    {
        return manifestService.FileNameMappings[fileName];
    }

    public static (int AttributeIndex, string RemappedFileName) RemapAttribute(
        this IViteManifestService manifestService,
        TagHelperAttributeList attributes,
        string htmlAttributeName)
    {
        int attributeIndex = attributes.IndexOfName(htmlAttributeName);
        var src = attributes[attributeIndex];
        if (src is null)
            throw new InvalidOperationException("client-src attribute is required");

        var srcValue = src.Value;
        string? srcStringValue;
        if (srcValue is null || (srcStringValue = srcValue.ToString()) is null)
            throw new InvalidOperationException("client-src attribute value is required");
        
        var remappedFileName = manifestService.RemapFileName(srcStringValue);
        return (attributeIndex, remappedFileName);
    }
}


/// <summary>
/// Path indicates The file to import. This is the file name as it appears in the vite manifest.
/// Aka as it is on disk in the source files of the client project.
/// 
/// NOTE: This thing is pretty fragile, because it doesn't have the manifest information at compile time.
///       Ideally, this mapping should happen as part of the build process, because otherwise it's very easy
///       to mess up the file name and so have errors only when running the production app.
///       There are multiple ways to do this, I just don't know what the idiomatic, or the simplest way is.
///       My ideas are either a source generator that looks at the manifest file and generates a class
///       or preprocessing the razor views.
/// </summary>
[HtmlTargetElement("ImportFile", Attributes = "path")]
public class ViteImportFileTagHelper : TagHelper
{
    private readonly IViteManifestService _manifestService;

    public ViteImportFileTagHelper(IViteManifestService manifestService)
    {
        _manifestService = manifestService;
    }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var (attributeIndex, remappedFileName) = _manifestService.RemapAttribute(output.Attributes, "path");
        output.Attributes.RemoveAt(attributeIndex);
        
        int dotIndex = remappedFileName.LastIndexOf('.') + 1;
        var extension = remappedFileName[dotIndex ..];
        var targetUrl = ProjectConfiguration.StaticFilesRoute + remappedFileName;

        switch (extension)
        {
            case "js":
            {
                output.TagName = "script";
                output.Attributes.SetAttribute("src", targetUrl);
                output.Attributes.SetAttribute("type", "module");
                break;
            }
            case "css":
            {
                output.TagName = "link";
                output.Attributes.SetAttribute("href", targetUrl);
                output.Attributes.SetAttribute("rel", "stylesheet");
                break;
            }
            default:
            {
                throw new InvalidOperationException($"Unknown file extension '.{extension}'");
            }
        }
    }
}