﻿using System;

namespace SourceGeneratorHelpers;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class GenerateIsTypeAttribute : Attribute
{
    public GenerateIsTypeAttribute(Type targetType) { }
}