using Microsoft.CodeAnalysis;
using SourceGeneratorHelpers;
using SourceGeneratorHelpersSourceGenerators.Generators;

namespace SourceGeneratorHelpersSourceGenerators;

// https://github.com/dotnet/roslyn-sdk/issues/850#issuecomment-1038725567
[Generator(LanguageNames.CSharp)]
public partial class SourceGeneratorHelpersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add Unconditionally generated files 
        context.RegisterPostInitializationOutput(StaticFiles);

        context.AddInterfaceHelperGenerator();
        context.AddIsTypeGenerator();
        context.AddEnumHelperGenerator();
    }

    public static void StaticFiles(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddStaticFile("Static.TypeHelpers.cs");
        context.AddStaticFile("Static.SyntaxHelpers.cs");
        context.AddStaticFile("Static.SymbolHelpers.cs");
        context.AddStaticFile("Static.TemplateHelpers.cs");
    }
}
