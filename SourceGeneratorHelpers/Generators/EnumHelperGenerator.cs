using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using SourceGeneratorHelpers;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SourceGeneratorHelpersSourceGenerators.Generators;

public static class EnumHelperGenerator
{
    public static void AddEnumHelperGenerator(this IncrementalGeneratorInitializationContext context)
    {
        // Add Unconditionally generated files 
        context.RegisterPostInitializationOutput(StaticFiles);

        // Search For Targets And Prepare Them
        var targetProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsTargetForGenerator,
                transform: PrepareDataForGeneration);

        // Filter Invalid
        targetProvider = targetProvider.Where(i => i is not null);

        // Generate the source
        context.RegisterSourceOutput(targetProvider, Generate);
    }

    public static void StaticFiles(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddStaticFile("Static.GenerateEnumHelperAttribute.cs");
    }

    public static bool IsTargetForGenerator(SyntaxNode SyntaxNode, CancellationToken cancellationToken)
    {
        if (SyntaxNode is not EnumDeclarationSyntax targetNode)
        {
            return false;
        }

        return targetNode.AttributeLists.Any();
    }

    public static INamedTypeSymbol PrepareDataForGeneration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken)!;

        var attr = symbol.GetAttributes();

        var filteredAttrs = attr.Where(item => item.AttributeClass!.Name == nameof(GenerateEnumHelperAttribute));

        if (!filteredAttrs.Any())
        {
            return null!;
        }

        return symbol;
    }

    public static void Generate(SourceProductionContext context, INamedTypeSymbol data)
    {
        var enumNamespace = data.ContainingNamespace.ToString();
        var enumName = data.Name;

        var sb = new StringBuilder();

        var members = data.GetMembers();

        foreach (var item in members)
        {
            if (item is not IFieldSymbol field)
            {
                continue;
            }

            if (!field.IsConst || !field.HasConstantValue)
            {
                continue;
            }

            // Only enum variants here
            sb.Indent(3).AppendLine($"{enumName}.{item.Name} => \"{enumName}.{item.Name}\",");
        }

        var replacements = new Dictionary<string, string>
        {
            { "EnumNamespace", enumNamespace },
            { "EnumName", enumName },
            { "FullNameContent", sb.ToString() },
        };

        var discriminator = data.ToString();

        context.AddTemplate("Templates.EnumHelper.cs", discriminator, replacements);
    }
}