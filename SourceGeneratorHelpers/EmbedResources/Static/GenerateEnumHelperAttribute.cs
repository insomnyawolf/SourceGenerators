using System;

namespace SourceGeneratorHelpers;

[AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
public sealed class GenerateEnumHelperAttribute : Attribute { }