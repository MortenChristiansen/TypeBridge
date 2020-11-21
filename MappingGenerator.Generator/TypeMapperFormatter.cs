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
            _sourceProperties = GetPropertiesRecursively(_sourceType).ToList();
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
            //System.Diagnostics.Debugger.Launch();

            if (destinationType.IsAbstract)
                return false;

            if (IsAssignable(_sourceType, destinationType))
                return true;

            var destinationProperties = GetPropertiesRecursively(destinationType).ToList();
            if (destinationProperties.Count > _sourceProperties.Count)
                return false;

            return destinationProperties.All(d => _sourceProperties.Any(s => d.Name.Equals(s.Name, StringComparison.InvariantCultureIgnoreCase) && IsAssignable(s.Type, d.Type)));
        }

        private IEnumerable<IPropertySymbol> GetPropertiesRecursively(ITypeSymbol type)
        {
            foreach (var property in type.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer && p.DeclaredAccessibility != Accessibility.Private))
                yield return property;

            if (type.BaseType != null)
            {
                foreach (var childProperty in GetPropertiesRecursively(type.BaseType))
                {
                    yield return childProperty;
                }
            }
        }

        private string FormatMapping(ITypeSymbol destinationType)
        {
            if (IsAssignable(_sourceType, destinationType))
                return "                m._source";

            var destinationProperties = GetPropertiesRecursively(destinationType).ToList();

            return
$@"            new {destinationType.GetQualifiedName()}()
            {{
                {string.Join($",{Environment.NewLine}                ", destinationProperties.Select(p => $"{p.Name} = m._source.{p.Name}"))}
            }}";
        }

        private bool IsAssignable(ITypeSymbol source, ITypeSymbol destination)
        {
            if (source.GetHashCode() == destination.GetHashCode())
                return true;

            if (source.Interfaces.Any(i => i == destination))
                return true;

            if (source.BaseType != null && IsAssignable(source.BaseType, destination))
                return true;

            return false;
        }
    }
}
