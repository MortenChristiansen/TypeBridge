﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingGenerator.Generator
{
    class TypeMapperFormatter
    {
        private readonly ITypeSymbol _sourceType;
        private readonly List<IPropertySymbol> _sourceProperties;
        private readonly List<ITypeSymbol> _destinationTypes = new List<ITypeSymbol>();

        public TypeMapperFormatter(ITypeSymbol sourceType)
        {
            _sourceType = sourceType;
            _sourceProperties = _sourceType.GetMembers().OfType<IPropertySymbol>().ToList();
        }

        public TypeMapperFormatter AddDestinationType(ITypeSymbol type)
        {
            if (!_destinationTypes.Any(t => t.GetHashCode() == type.GetHashCode()) && CanMap(type)) // TODO: Find more robust comparison
                _destinationTypes.Add(type);

            return this;
        }

        public string Format() =>
$@"class {_sourceType.Name}_Mapper
    {{
        {string.Join($"{Environment.NewLine}        ", _sourceType.GetMembers().OfType<IPropertySymbol>().Select(p => $"public {p.Type} {p.Name} {{ get; }}"))}

        public {_sourceType.Name}_Mapper({_sourceType.Name} v)
        {{
            {string.Join($"{Environment.NewLine}            ", _sourceType.GetMembers().OfType<IPropertySymbol>().Select(p => $"{p.Name} = v.{p.Name};"))}
        }}

        {string.Join(Environment.NewLine, _destinationTypes.Select(FormatImplicitOperator))}
    }}";

        private string FormatImplicitOperator(ITypeSymbol destinationType) =>
$@"public static implicit operator {destinationType.GetQualifiedName()}({_sourceType.Name}_Mapper m) =>
{FormatMapping(destinationType)};";

        private bool CanMap(ITypeSymbol destinationType)
        {
            var destinationProperties = destinationType.GetMembers().OfType<IPropertySymbol>().ToList();
            if (destinationProperties.Count != _sourceProperties.Count)
                return false;

            // TODO: There must be a constructor that we can use to create the instance
            // TODO: More robust implementation - maybe even detect if the type can be assigned to the destination implicitly (or via base class)
            return _sourceProperties.All(s => destinationProperties.Any(d => s.Name.Equals(d.Name, StringComparison.InvariantCultureIgnoreCase) && s.Type.Name == d.Type.Name));
        }

        private string FormatMapping(ITypeSymbol destinationType)
        {
            var destinationProperties = destinationType.GetMembers().OfType<IPropertySymbol>().ToList();

            return
$@"            new {destinationType.GetQualifiedName()}()
            {{
                {string.Join($",{Environment.NewLine}                ", destinationProperties.Select(p => $"{p.Name} = m.{p.Name}"))}
            }}";
        }
    }
}