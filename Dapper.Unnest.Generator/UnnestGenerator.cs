using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Dapper.Unnest.Generator;

[Generator]
public class UnnestGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetClassToGenerate(ctx))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(provider, static (ctx, source) => Execute(ctx, source!));
    }

    private static ClassToGenerate? GetClassToGenerate(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeName = attribute.Name.ToString();
                if (attributeName == "Unnestable" || attributeName == "UnnestableAttribute")
                {
                    var semanticModel = context.SemanticModel;
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
                    if (classSymbol is not null)
                    {
                        var properties = classSymbol.GetMembers()
                            .OfType<IPropertySymbol>()
                            .Where(p => p.DeclaredAccessibility == Accessibility.Public && p.GetMethod is not null && p.SetMethod is not null)
                            .Select(p => new PropertyInfo(p.Name, p.Type.ToDisplayString()))
                            .ToImmutableArray();

                        var fullName = classSymbol.ToDisplayString();
                        return new ClassToGenerate(classSymbol.Name, classSymbol.ContainingNamespace.ToDisplayString(), fullName, properties);
                    }
                }
            }
        }

        return null;
    }

    private static void Execute(SourceProductionContext context, ClassToGenerate classToGenerate)
    {
        var source = GenerateSource(classToGenerate);
        context.AddSource($"{classToGenerate.Name}Unnested.g.cs", source);
    }

    private static string GenerateSource(ClassToGenerate classToGenerate)
    {
        var namespaceName = classToGenerate.Namespace;
        var className = classToGenerate.Name;
        var unnestedClassName = $"{className}Unnested";

        var properties = string.Join("\n", classToGenerate.Properties.Select(p =>
            $"    public {p.Type}[] {p.Name} {{ get; set; }} = Array.Empty<{p.Type}>();"));

        var extensionMethod = GenerateExtensionMethod(classToGenerate, namespaceName);

        return $@"
namespace {namespaceName};

public class {unnestedClassName}
{{
{properties}
}}

public static class {className}UnnestExtensions
{{
{extensionMethod}
}}
";
    }

    private static string GenerateExtensionMethod(ClassToGenerate classToGenerate, string namespaceName)
    {
        var className = classToGenerate.Name;
        var unnestedClassName = $"{className}Unnested";

        var arrays = string.Join("\n", classToGenerate.Properties.Select(p =>
            $"        var {p.Name.ToLower()}Array = new {p.Type}[count];"));

        var adds = string.Join("\n", classToGenerate.Properties.Select(p =>
            $"            {p.Name.ToLower()}Array[i] = item.{p.Name};"));

        var assignments = string.Join("\n", classToGenerate.Properties.Select(p =>
            $"        result.{p.Name} = {p.Name.ToLower()}Array;"));

        return $@"
    public static {unnestedClassName} ToUnnested(this System.Collections.Generic.IEnumerable<{className}> source, int count)
    {{
{arrays}

        int i = 0;
        foreach (var item in source)
        {{
{adds}
            i++;
        }}

        var result = new {unnestedClassName}();
{assignments}

        return result;
    }}

    public static {unnestedClassName} ToUnnested(this System.Collections.Generic.IReadOnlyCollection<{className}> source)
    {{
        return source.ToUnnested(source.Count);
    }}";
    }
}

public record ClassToGenerate(string Name, string Namespace, string FullName, ImmutableArray<PropertyInfo> Properties);

public record PropertyInfo(string Name, string Type);
