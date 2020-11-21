using Microsoft.CodeAnalysis;
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

        public bool HasMappings => _destinationTypes.Count > 0;

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
$@"sealed class {_sourceType.Name}_Mapper
    {{
        private readonly {_sourceType.Name} _source;

        public {_sourceType.Name}_Mapper({_sourceType.Name} source)
        {{
            _source = source;
        }}

        {string.Join(Environment.NewLine, _destinationTypes.Select(FormatImplicitOperator))}
    }}";

        private string FormatImplicitOperator(ITypeSymbol destinationType) =>
$@"public static implicit operator {destinationType.GetQualifiedName()}({_sourceType.Name}_Mapper m) =>
{FormatMapping(destinationType)};";

        private bool CanMap(ITypeSymbol destinationType)
        {
            var destinationProperties = destinationType.GetMembers().OfType<IPropertySymbol>().ToList();
            if (destinationProperties.Count > _sourceProperties.Count)
                return false;

            // TODO: More robust implementation
            
            
            return destinationProperties.All(d => _sourceProperties.Any(s => d.Name.Equals(s.Name, StringComparison.InvariantCultureIgnoreCase) && d.Type.GetHashCode() == s.Type.GetHashCode()));
        }

        private string FormatMapping(ITypeSymbol destinationType)
        {
            var destinationProperties = destinationType.GetMembers().OfType<IPropertySymbol>().ToList();

            return
$@"            new {destinationType.GetQualifiedName()}()
            {{
                {string.Join($",{Environment.NewLine}                ", destinationProperties.Select(p => $"{p.Name} = m._source.{p.Name}"))}
            }}";
        }
    }
}
