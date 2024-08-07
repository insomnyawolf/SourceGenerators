using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace SourceGeneratorHelpers;

public static partial class SymbolHelpers
{
    public static IEnumerable<ITypeSymbol> GetTemplateNameTemplate(this ITypeSymbol symbol)
    {
        var FQDN = typeof(TemplateTypeTemplate).GetFullyQualifiedName();
        return symbol.GetInterfaces(FQDN);
    }

    public static bool IsTemplateNameTemplate(this ITypeSymbol symbol)
    {
        return symbol.GetTemplateNameTemplate().Any();
    }
}
