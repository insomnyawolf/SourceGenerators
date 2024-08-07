using System;

namespace SourceGeneratorHelpers;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class GenerateInterfaceHelperAttribute : Attribute
{
    public GenerateInterfaceHelperAttribute(Type targetType) { }
}