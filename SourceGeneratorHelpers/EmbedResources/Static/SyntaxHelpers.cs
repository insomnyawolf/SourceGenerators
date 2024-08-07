#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace SourceGeneratorHelpers;

public static class SyntaxHelpers
{
    public static string GetAttributeName<TClass>() where TClass : Attribute
    {
        var type = typeof(TClass);

        var name = type.Name;

        const string attrSuf = nameof(Attribute);

        if (name.EndsWith(attrSuf))
        {
            name = name.Substring(0, name.Length - attrSuf.Length);
        }

        return name;
    }

    public static bool HasToken(this SyntaxTokenList tokenList, string tokenValue)
    {
        for (int i = 0; i < tokenList.Count; i++)
        {
            var item = tokenList[i];

            if (item.Text == tokenValue)
            {
                return true;
            }
        }

        return false;
    }

    public static bool TryGetAttribute(this MemberDeclarationSyntax member, string name, out AttributeSyntax? AttributeSyntax)
    {
        foreach (var attrList in member.AttributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                if (attr.Name.ToString() == name)
                {
                    AttributeSyntax = attr;
                    return true;
                }
            }
        }

        AttributeSyntax = null;
        return false;
    }

    public static bool HasInterface(this ClassDeclarationSyntax @class, string name)
    {
        var baselist = @class.BaseList;

        if (baselist is null)
        {
            return false;
        }

        var types = baselist.Types;

        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
            var typeName = type.ToString();
            if (typeName == name)
            {
                return true;
            }
        }

        return false;
    }
}
