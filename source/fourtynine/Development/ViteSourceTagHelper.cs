using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace fourtynine;

public class ViteManifestService : IViteManifestService
{
    public IReadOnlyDictionary<string, string> FileNameMappings { get; }

    public ViteManifestService(string manifestPath)
    {
        var manifest = File.ReadAllText(manifestPath);
        var manifestJson = JsonDocument.Parse(manifest);
        FileNameMappings = manifestJson.RootElement
            .EnumerateObject()
            .ToDictionary(x => x.Name, x => x.Value.GetProperty("file").GetString()!);
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

[HtmlTargetElement("StaticFile", Attributes = "source")]
public class ViteStaticFileTagHelper : TagHelper
{
    private readonly IViteManifestService _manifestService;

    public ViteStaticFileTagHelper(IViteManifestService manifestService)
    {
        _manifestService = manifestService;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var (attributeIndex, remappedFileName) = _manifestService.RemapAttribute(output.Attributes, "manifest-url");
        output.Attributes.RemoveAt(attributeIndex);
        
        var dotIndex = remappedFileName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1;
        var extension = remappedFileName[dotIndex ..];

        switch (extension)
        {
            case "js":
            {
                output.TagName = "script";
                output.Attributes.SetAttribute("src", remappedFileName);
                output.Attributes.SetAttribute("type", "module");
                break;
            }
            case "css":
            {
                output.TagName = "link";
                output.Attributes.SetAttribute("href", remappedFileName);
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