using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                predicate: static (s, _) => IsClassOrRecordDeclaration(s),
                transform: static (ctx, _) => GetClassOrRecordToGenerate(ctx))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(provider, static (ctx, source) => Execute(ctx, source!));
    }

    private static bool IsClassOrRecordDeclaration(SyntaxNode node)
    {
        return node is TypeDeclarationSyntax typeDeclaration &&
               (typeDeclaration.IsKind(SyntaxKind.ClassDeclaration) ||
                typeDeclaration.IsKind(SyntaxKind.RecordDeclaration) ||
                typeDeclaration.IsKind(SyntaxKind.RecordStructDeclaration));
    }

    private static ClassToGenerate? GetClassOrRecordToGenerate(GeneratorSyntaxContext context)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;

        foreach (var attributeList in typeDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeName = attribute.Name.ToString();
                if (attributeName == "Unnestable" || attributeName == "UnnestableAttribute" ||
                    attributeName == "Unnested" || attributeName == "UnnestedAttribute")
                {
                    var semanticModel = context.SemanticModel;
                    var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
                    if (typeSymbol is not null)
                    {
                        var properties = GetProperties(typeSymbol);

                        var accessibility = typeSymbol.DeclaredAccessibility;

                        return new ClassToGenerate(
                            typeSymbol.Name,
                            typeSymbol.ContainingNamespace.ToDisplayString(),
                            properties,
                            accessibility);
                    }
                }
            }
        }

        return null;
    }

    private static ImmutableArray<PropertyInfo> GetProperties(INamedTypeSymbol typeSymbol)
    {
        var properties = new List<PropertyInfo>();

        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IPropertySymbol propertySymbol &&
                propertySymbol.DeclaredAccessibility == Accessibility.Public &&
                propertySymbol.GetMethod is not null && 
                !propertySymbol.IsIndexer)
            {
                properties.Add(new PropertyInfo(propertySymbol.Name, propertySymbol.Type.ToDisplayString()));
            }
        }
         
        return [..properties];
    }

    private static void Execute(SourceProductionContext context, ClassToGenerate classToGenerate)
    {
        var source = GenerateSource(classToGenerate);
        context.AddSource($"{classToGenerate.Name}Unnestable.g.cs", source);
    }

    private static string GenerateSource(ClassToGenerate classToGenerate)
    {
        var classModifiers = GetAccessibilityModifiers(classToGenerate.Accessibility);

        var propertiesCode = string.Join("\n",
            classToGenerate.Properties.Select(p =>
                $"    public {p.Type}[] {p.Name} {{ get; set; }} = Array.Empty<{p.Type}>();"));

        var className = classToGenerate.Name;
        var unnestableClassName = $"{className}Unnestable";

        var arrayDeclarations = string.Join("\n",
            classToGenerate.Properties.Select(p =>
            {
                var isArray = p.Type.EndsWith("[]");
                var elementType = isArray ? p.Type.Replace("[]", "") : p.Type;
                var arrayType = isArray ? $"{elementType}[count][]" : $"{p.Type}[count]";
                return $"        var {p.Name.ToLower()}Array = new {arrayType};";
            }));

        var arrayAssignments = string.Join("\n",
            classToGenerate.Properties.Select(p =>
                $"            {p.Name.ToLower()}Array[i] = item.{p.Name};"));

        var resultAssignments = string.Join("\n",
            classToGenerate.Properties.Select(p =>
                $"        result.{p.Name} = {p.Name.ToLower()}Array;"));

        return $$"""
                 #nullable restore

                 namespace {{classToGenerate.Namespace}};

                 {{classModifiers}} sealed class {{unnestableClassName}}
                 {
                 {{propertiesCode}}
                 }

                 {{classModifiers}} static class {{className}}UnnestExtensions
                 {
                     public static {{unnestableClassName}} ToUnnestable(this System.Collections.Generic.IEnumerable<{{className}}> source, int count)
                     {
                 {{arrayDeclarations}}

                         int i = 0;

                         foreach (var item in source)
                         {
                 {{arrayAssignments}}

                             i++;
                         }

                         var result = new {{unnestableClassName}}();

                 {{resultAssignments}}

                         return result;
                     }

                     public static {{unnestableClassName}} ToUnnestable(this System.Collections.Generic.IReadOnlyCollection<{{className}}> source)
                     {
                         return source.ToUnnestable(source.Count);
                     }
                 }
                 """;
    }

    private static string GetAccessibilityModifiers(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => "internal" // fallback
        };
    }
}

public record ClassToGenerate(
    string Name,
    string Namespace,
    ImmutableArray<PropertyInfo> Properties,
    Accessibility Accessibility = Accessibility.Public);

public record PropertyInfo(string Name, string Type);
