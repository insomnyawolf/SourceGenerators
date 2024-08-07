#nullable enable

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SourceGeneratorHelpers;

[GenerateInterfaceHelper(typeof(IEnumerable<>))]
[GenerateInterfaceHelper(typeof(IEquatable<>))]
[GenerateInterfaceHelper(typeof(IComparable<>))]
[GenerateInterfaceHelper(typeof(IDictionary<,>))]
[GenerateIsType(typeof(bool))]
[GenerateIsType(typeof(byte))]
[GenerateIsType(typeof(string))]
[GenerateIsType(typeof(Enum))]
[GenerateIsType(typeof(TimeSpan))]
[GenerateIsType(typeof(DateTime))]
[GenerateIsType(typeof(DateTimeOffset))]
public static partial class SymbolHelpers
{
    public static string GetFullyQualifiedName(this ISymbol symbol)
    {
        return symbol.ToDisplayString();
    }

    /// <summary>
    /// The GlobalNamespace contains every symbol referenced by the current project
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static INamespaceSymbol GetGlobalNamespace(this INamespaceSymbol symbol)
    {
        while (!symbol.IsGlobalNamespace) 
        {
            symbol = symbol.ContainingNamespace;
        }

        return symbol;
    }

    public static bool IsOrInheritFrom(this ITypeSymbol symbol, string fullyQualifiedName)
    {
        if (symbol.Is(fullyQualifiedName))
        {
            return true;
        }

        if (symbol.InheritFrom(fullyQualifiedName))
        {
            return true;
        }

        return false;
    }

    public static bool Is(this ITypeSymbol symbol, string fullyQualifiedName)
    {
        // If this fails either there's 2 things with the same name and we would need to check the namespace
        // Or the type doesn't exist in the compilation yet, be extremately careful with that last one
        var name = symbol.OriginalDefinition.ToString();

        if (name == fullyQualifiedName)
        {
            return true;
        }

        return false;
    }

    public static bool InheritFrom(this ITypeSymbol symbol, string fullyQualifiedName)
    {
        while (symbol.BaseType is not null)
        {
            symbol = symbol.BaseType;

            if (symbol.Is(fullyQualifiedName))
            {
                return true;
            }
            
        }

        return false;
    }

    public static bool IsIAsyncResult(this ITypeSymbol symbol)
    {
        return symbol.AllInterfaces.Any(i => i.Name == typeof(IAsyncResult).Name);
    }

    public static IEnumerable<ITypeSymbol> GetInterfaces(this ITypeSymbol symbol, string name)
    {
        if (symbol is INamedTypeSymbol named)
        {
            if (named.IsNullable())
            {
                symbol = named.TypeArguments[0];
            }
        }

        if (symbol.TypeKind == TypeKind.Interface)
        {
            var orgStr = symbol.GetOriginalTypeDefinitionName();

            if (orgStr == name)
            {
                yield return symbol;
            }
        }

        var results = symbol.AllInterfaces.Where(i => 
        {
            var orgStr = i.GetOriginalTypeDefinitionName();
            return orgStr == name;
        });

        foreach (var result in results)
        {
            yield return result;
        }
    }

    public static string GetOriginalTypeDefinitionName(this ITypeSymbol symbol)
    {
        var org = symbol.OriginalDefinition;
        var orgStr = org.ToString();
        return orgStr;
    }

    public static ITypeSymbol GetUnderlyingNullableTypeOrCurrent(this ITypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol named)
        {
            if (symbol.IsReferenceType)
            {
                return named.ConstructedFrom;
            }

            if (symbol.IsNullable())
            {
                return named.TypeArguments[0];
            }
        }

        return symbol;
    }

    public static string GetUnderlyingNullableNameOrCurrent(this ITypeSymbol symbol)
    {
        symbol = symbol.GetUnderlyingNullableTypeOrCurrent();

        var name = symbol.GetFullyQualifiedName();

        return name;
    }

    public static bool IsNullable(this ITypeSymbol symbol)
    {
        return symbol.ContainingNamespace?.Name == "System" && symbol.Name == "Nullable";
    }

    public static bool IsPartiallyUdaptableClass(this ITypeSymbol symbol)
    {
        return symbol.TypeKind == TypeKind.Class
            && !symbol.IsString()
            && !symbol.IsIEnumerable();
    }

    public static bool IsInSystemNamespace(this ITypeSymbol symbol)
    {
#warning this may be buggy
        return symbol.IsInNamespace("System");
    }

    public static bool IsInNamespace(this ITypeSymbol symbol, string @namespace)
    {
        var ns = symbol.ContainingNamespace.ToDisplayString();
        return ns.StartsWith(@namespace);
    }

    public static bool IsClass(this ITypeSymbol symbol)
    {
        return symbol.TypeKind == TypeKind.Class;
    }

    public static bool HasAttribute(this ITypeSymbol typeSymbol, string name)
    {
        var attrs = typeSymbol.GetAttributes();
        foreach (var attr in attrs)
        {
            if (attr.AttributeClass?.Name == name)
            {
                return true;
            }
        }

        return false;
    }
}
