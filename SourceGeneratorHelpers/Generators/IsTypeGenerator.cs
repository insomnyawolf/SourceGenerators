using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using SourceGeneratorHelpers;
using System.Linq;
using System.Collections.Generic;

namespace SourceGeneratorHelpersSourceGenerators.Generators;

public static class IsTypeGenerator
{
    public static void AddIsTypeGenerator(this IncrementalGeneratorInitializationContext context)
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
        context.AddStaticFile("Static.GenerateIsTypeAttributte.cs");
    }

    public static bool IsTargetForGenerator(SyntaxNode SyntaxNode, CancellationToken cancellationToken)
    {
        if (SyntaxNode is not ClassDeclarationSyntax methodNode)
        {
            return false;
        }

        return methodNode.AttributeLists.Any();
    }

    public static IEnumerable<Helper> PrepareDataForGeneration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken)!;

        var attr = symbol.GetAttributes();

        var filteredAttrs = attr.Where(item => item.AttributeClass!.Name == nameof(GenerateIsTypeAttribute));

        if (!filteredAttrs.Any())
        {
            return null!;
        }

        var interestingData = filteredAttrs.Select(item => item.ConstructorArguments[0].Value).Cast<INamedTypeSymbol>();

        var items = interestingData.Select(i => new Helper(i));

        return items;
    }

    public class Helper
    {
        public readonly string Name;
        public readonly string Comparator;

        public Helper(INamedTypeSymbol namedTypeSymbol)
        {
            this.Name = namedTypeSymbol.Name;
            this.Comparator = namedTypeSymbol.ToString();
        }
    }

    public static void Generate(SourceProductionContext context, IEnumerable<Helper> helpers)
    {
        foreach (var item in helpers)
        {
            var replacements = new Dictionary<string, string>
            {
                { "Name", item.Name },
                { "Comparator", item.Comparator },
            };

            context.AddTemplate("Templates.IsType.cs", item.Comparator, replacements);
        }
    }
}