using System.IO;
using System.Linq;
using System.Text;
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
            
            // Hack to generate reflection info.
            // There's no documentation on this on the NSwag github wiki,
            // and the code doesn't seem particularly extensible.
            StringBuilder builder = new();
            builder.AppendLine(code);
            {
                builder.AppendLine("export const ApiPropertyTable = {");
                
                // The json schemas don't seem to be aware of themselves, aka
                // they don't contain their own name or anything like that.
                var reverseMap = document.Definitions
                    .Where(x => x.Value.Type == JsonObjectType.Object)
                    .ToDictionary(x => x.Value, x => x.Key);
                
                foreach (var (schemaName, schema) in document.Definitions)
                {
                    // I have a code builder utility, but I didn't feel like bringing it in.
                    builder.AppendLine($"    {schemaName}: [");
                    foreach (var (propertyName, property) in schema.Properties)
                    {
                        var propSchema = property.ActualSchema;
                        string typeNameText = reverseMap.TryGetValue(propSchema, out string typeName)
                            ? $"\"{typeName}\""
                            : "null";
                        builder.Append("        ");
                        builder.AppendLine($"{{ name: \"{propertyName}\", schemaTypeName: {typeNameText} }},");
                    }
                    builder.AppendLine("    ],");
                }
                builder.AppendLine("};");
            }
            
            var outputFilePath = ViteDirectory / "src" / "api-client.ts";
            File.WriteAllText(outputFilePath, builder.ToString());
        });
}

class KeepNamesPropertyNameGenerator : TypeScriptPropertyNameGenerator
{
    public static readonly KeepNamesPropertyNameGenerator Instance = new();
    public override string Generate(JsonSchemaProperty property) => property.Name;
}