using Microsoft.AspNetCore.Hosting;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;
using System.Text.RegularExpressions;
using tree_api.Tests.ClientGenerator.Utils;


var factory = new CustomWebApplicationFactory().WithWebHostBuilder(webHostBuilder =>
{
    webHostBuilder.UseEnvironment("Local");
    webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
});
var server = factory.Server;
using var client = server.CreateClient();
var document = await OpenApiDocument.FromJsonAsync(await client.GetStringAsync("swagger/v1/swagger.json"));

var settings = new CSharpClientGeneratorSettings
{
    ClassName = "TreeAppClient",
    CSharpGeneratorSettings =
    {
        Namespace = "tree.api.Tests.ClientGenerator",
        RequiredPropertiesMustBeDefined = false,
        GenerateOptionalPropertiesAsNullable = true,
        GenerateNullableReferenceTypes = true
    },
    OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator(),
    ExposeJsonSerializerSettings = true
};

var generator = new CSharpClientGenerator(document, settings);
var code = generator.GenerateFile();
var path = Regex.Replace(AppDomain.CurrentDomain.BaseDirectory, "bin.*", "");
path = Path.Combine(path, "Client/TreeAppClient.cs");
await File.WriteAllTextAsync(path, code);
