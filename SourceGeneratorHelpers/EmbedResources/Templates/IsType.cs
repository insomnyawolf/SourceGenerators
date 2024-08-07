using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace SourceGeneratorHelpers;

public static partial class SymbolHelpers
{
    public static bool IsTemplateNameTemplate(this ITypeSymbol symbol)
    {
        // return symbol.ContainingNamespace.Name == "System" && symbol.Name == "Boolean";
        return symbol.IsOrInheritFrom("TemplateComparatorTemplate");
    }
}
