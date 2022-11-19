using System.IO;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag;
using NSwag.CodeGeneration.TypeScript;
using Nuke.Common;

partial class Build
{
    Target GenerateSwaggerTypeScriptClient => _ => _
        .Executes(async () =>
        {
            // TODO: share this bit of configuration
            const string swaggerJsonUrl = "https://localhost:7186/swagger/v1/swagger.json";
            var document = await OpenApiDocument.FromUrlAsync(swaggerJsonUrl);

            var settings = new TypeScriptClientGeneratorSettings();
            settings.CodeGeneratorSettings.PropertyNameGenerator = KeepNamesPropertyNameGenerator.Instance;

            var generator = new TypeScriptClientGenerator(document, settings);
            var code = generator.GenerateFile();
            
            var outputFilPath = ViteDirectory / "src" / "api-client.ts";
            File.WriteAllText(outputFilPath, code);
        });
}

internal class KeepNamesPropertyNameGenerator : TypeScriptPropertyNameGenerator
{
    public static readonly KeepNamesPropertyNameGenerator Instance = new();
    public override string Generate(JsonSchemaProperty property) => property.Name;
}