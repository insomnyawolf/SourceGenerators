#nullable enable

using Microsoft.CodeAnalysis;
using System;
using System.Reflection;

namespace SourceGeneratorHelpers;

public static partial class TypeHelpers
{
    public static string RemoveGenericPartOfTypeName(string name)
    {
        var indexOfGeneric = name.IndexOf('`');

        if (indexOfGeneric < 0)
        {
            // No generics to remove
            return name;
        }

        var newName = name.Substring(0, indexOfGeneric);

        return newName;
    }

    public static string GetFullyQualifiedName(this Type symbol)
    {
        var name = symbol.ToString();

        var typeInfo = symbol.GetTypeInfo();

        if (typeInfo.GenericTypeParameters.Length < 1)
        {
            return name;
        }

        name = RemoveGenericPartOfTypeName(name);

        name += '<';

        for (int i = 0; i < typeInfo.GenericTypeParameters.Length; i++)
        {
            if (i > 0)
            {
                name += ", ";
            }

            var param = typeInfo.GenericTypeParameters[i];

            name += param.Name;
        }

        name += '>';

        return name;
    }
}
