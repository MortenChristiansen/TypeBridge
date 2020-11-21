using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingGenerator.Generator
{
    class TypeMapperFormatter
    {
        private readonly ITypeSymbol _sourceType;
        private readonly List<ITypeSymbol> _destinationTypes = new List<ITypeSymbol>();
        private Dictionary<ITypeSymbol, List<IPropertySymbol>> _properties = new Dictionary<ITypeSymbol, List<IPropertySymbol>>();

        public bool HasMappings => _destinationTypes.Count > 0;

        public TypeMapperFormatter(ITypeSymbol sourceType)
        {
            _sourceType = sourceType;
        }

        public TypeMapperFormatter AddDestinationType(ITypeSymbol type)
        {
            if (!_destinationTypes.Any(t => t.GetHashCode() == type.GetHashCode()) && IsMappable(_sourceType, type))
                _destinationTypes.Add(type);

            return this;
        }

        // IArrayTypeSymbol

        private bool IsMappable(ITypeSymbol sourceType, ITypeSymbol destinationType)
        {
           // System.Diagnostics.Debugger.Launch();

            if (destinationType.IsAbstract && destinationType.TypeKind == TypeKind.Class)
                return false;

            if (IsAssignable(sourceType, destinationType))
                return true;

            // Also suppor any combination of IEnumerable and List
            if (TryGetCollectionType(sourceType, out var sourceElementType, out var _) && TryGetCollectionType(destinationType, out var destinationElementType, out var _))
            {
                if (IsMappable(sourceElementType, destinationElementType))
                    return true;

                return false;
            }

            var sourceProperties = GetPropertiesRecursively(sourceType);
            var destinationProperties = GetPropertiesRecursively(destinationType);
            if (destinationProperties.Count > sourceProperties.Count)
                return false;

            return destinationProperties.All(d => sourceProperties.Any(s => d.Name == s.Name && IsMappable(s.Type, d.Type)));
        }

        private bool TryGetCollectionType(ITypeSymbol type, out ITypeSymbol elementType, out string postfix)
        {
            var names = new[] { "List", "IEnumerable" };

            if (names.Contains(type.Name) && type is INamedTypeSymbol list && list.TypeArguments.Length == 1)
            {
                elementType = list.TypeArguments[0];
                postfix = ".ToList()";
                return true;
            }
            else if (type is IArrayTypeSymbol arraySymbol)
            {
                elementType = arraySymbol.ElementType;
                postfix = ".ToArray()";
                return true;
            }
            else
            {
                elementType = null;
                postfix = null;
                return false;
            }
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

        public string Format() =>
$@"sealed class {_sourceType.Name}_Mapper
    {{
        private readonly {_sourceType.Name} _source;

        public {_sourceType.Name}_Mapper({_sourceType.Name} source)
        {{
            _source = source;
        }}

        {string.Join(Environment.NewLine, _destinationTypes.Select(d => FormatImplicitOperator(_sourceType, d)))}
    }}";

        private string FormatImplicitOperator(ITypeSymbol sourceType, ITypeSymbol destinationType) =>
$@"public static implicit operator {destinationType.GetQualifiedName()}({sourceType.Name}_Mapper m) =>
{FormatMapping("m._source", sourceType, destinationType, 1)};";

        private List<IPropertySymbol> GetPropertiesRecursively(ITypeSymbol type)
        {
            IEnumerable<IPropertySymbol> EnumerateAllProperties(ITypeSymbol t)
            {
                foreach (var property in t.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer && p.DeclaredAccessibility != Accessibility.Private))
                    yield return property;

                if (t.BaseType != null)
                {
                    foreach (var childProperty in GetPropertiesRecursively(t.BaseType))
                    {
                        yield return childProperty;
                    }
                }
            }

            if (!_properties.ContainsKey(type))
                _properties.Add(type, EnumerateAllProperties(type).ToList());

            return _properties[type];
        }

        private string FormatMapping(string sourceName, ITypeSymbol sourceType, ITypeSymbol destinationType, int nextListNameNumber)
        {
            //System.Diagnostics.Debugger.Launch();

            if (IsAssignable(sourceType, destinationType))
                return $"                {sourceName}";

            if (TryGetCollectionType(sourceType, out var sourceElementType, out var _) && TryGetCollectionType(destinationType, out var destinationElementType, out var postfix))
            {
                return $"{sourceName}.Select(x{nextListNameNumber} => {FormatMapping($"x{nextListNameNumber++}", sourceElementType, destinationElementType, nextListNameNumber)}){postfix}";
            }

            var sourceProperties = GetPropertiesRecursively(sourceType);
            var destinationProperties = GetPropertiesRecursively(destinationType);

            return
$@"            new {destinationType.GetQualifiedName()}()
            {{
                {string.Join($",{Environment.NewLine}                ", destinationProperties.Select(dp => FormatPropertyMapping(sourceName, sourceProperties.Single(sp => sp.Name == dp.Name).Type, dp, nextListNameNumber)))}
            }}";
        }

        private string FormatPropertyMapping(string sourceName, ITypeSymbol sourceType, IPropertySymbol destinationProperty, int nextListNameNumber)
        {
            if (IsAssignable(sourceType, destinationProperty.Type))
                return $"{destinationProperty.Name} = {sourceName}.{destinationProperty.Name}";

            return $"{destinationProperty.Name} = {FormatMapping($"{sourceName}.{destinationProperty.Name}", sourceType, destinationProperty.Type, nextListNameNumber)}";
        }
    }
}
