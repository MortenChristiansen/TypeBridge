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
        private List<(ITypeSymbol Destination, ITypeSymbol[] Extensions)> _extensionTypes = new List<(ITypeSymbol Destination, ITypeSymbol[] Extensions)>();

        public bool HasMappings => _destinationTypes.Count > 0;

        public TypeMapperFormatter(ITypeSymbol sourceType)
        {
            _sourceType = sourceType;
        }

        public TypeMapperFormatter AddDestinationType(ITypeSymbol type, ITypeSymbol[] extensionTypes)
        {
            if (!_destinationTypes.Any(t => t.GetHashCode() == type.GetHashCode()) && IsMappable(_sourceType, type, extensionTypes))
                _destinationTypes.Add(type);

            if (extensionTypes.Length > 0 && !_extensionTypes.Any(e => IsSame(e.Extensions, extensionTypes)))
                _extensionTypes.Add((type, extensionTypes));

            return this;
        }

        public bool IsSame(ITypeSymbol[] types1, ITypeSymbol[] types2)
        {
            if (types1.Length != types2.Length)
                return false;

            for (int i = 0; i < types1.Length; i++)
            {
                if (types1[i].GetHashCode() != types2[i].GetHashCode())
                    return false;
            }

            return true;
        }

        private bool IsMappable(ITypeSymbol sourceType, ITypeSymbol destinationType, ITypeSymbol[] extensionTypes, int level = 1)
        {
            // Simple recursion guard
            if (level > 10) // TODO: Make more intelligent recursion guard
                return false;

            //System.Diagnostics.Debugger.Launch();

            if (destinationType.IsAbstract && destinationType.TypeKind == TypeKind.Class)
                return false;

            if (IsAssignable(sourceType, destinationType))
                return true;

            // Also suppor any combination of IEnumerable and List
            if (TryGetCollectionType(sourceType, out var sourceElementType, out var _) && TryGetCollectionType(destinationType, out var destinationElementType, out var _))
            {
                if (IsMappable(sourceElementType, destinationElementType, extensionTypes, level + 1))
                    return true;

                return false;
            }

            var sourceProperties = GetCombinedSourceProperties("m._source", sourceType, extensionTypes);
            var destinationProperties = GetPropertiesRecursively(destinationType);
            if (destinationProperties.Count > sourceProperties.Count)
                return false;

            return destinationProperties.All(d => sourceProperties.Any(s => sourceProperties.ContainsKey(d.Name) && IsMappable(sourceProperties[d.Name].Type, d.Type, extensionTypes, level + 1)));
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
        private readonly {_sourceType.ToDisplayString()} _source;

        public {_sourceType.Name}_Mapper({_sourceType.ToDisplayString()} source)
        {{
            _source = source;
        }}

        {FormatExtendMethods()}

        {string.Join(Environment.NewLine, _destinationTypes.Where(d => IsMappable(_sourceType, d, new ITypeSymbol[0])).Select(d => FormatImplicitOperator($"{_sourceType.Name}_Mapper m", _sourceType, d, new ITypeSymbol[0])))}

{string.Join(Environment.NewLine + Environment.NewLine, _extensionTypes.Select((e, i) => FormatExensionMapper(i+1, e.Extensions)))}
    }}";

        private string FormatExtendMethods()
        {
            //System.Diagnostics.Debugger.Launch();

            var methodComment =
@"/// <summary>
        /// Add additional source fields to the mapping. The properties for additional
        /// extensions each add to the mapping potential. If multiple sources share a
        /// property, the last extension will be the one which gets mapped for the
        /// property.
        /// </summary>
        /// <param name=""value"">An additional object to map properties from</param>";

            if (_extensionTypes.Count == 0)
                return
$@"{methodComment}
        public {_sourceType.Name}_Mapper Extend(object value) =>
            this;
";

            return string.Join(Environment.NewLine + Environment.NewLine, _extensionTypes.Select((t, i) =>
$@"       {methodComment}
        public {_sourceType.Name}_Mapper.Extension{i+1} Extend({string.Join(", ", t.Extensions.Select((v, i2) => $"{v.GetQualifiedName()} v{i2+1}"))}) =>
            new {_sourceType.Name}_Mapper.Extension{i + 1}(_source, {string.Join(", ", t.Extensions.Select((_, i2) => $"v{i2 + 1}"))});
"));
        }

        private string FormatImplicitOperator(string mapperTypeName, ITypeSymbol sourceType, ITypeSymbol destinationType, ITypeSymbol[] extensions) =>
$@"public static implicit operator {destinationType.ToDisplayString()}({mapperTypeName}) =>
{FormatMapping("m._source", sourceType, destinationType, 1, extensions)};";

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

        private string FormatMapping(string sourceName, ITypeSymbol sourceType, ITypeSymbol destinationType, int nextListNameNumber, ITypeSymbol[] extensions)
        {
            //System.Diagnostics.Debugger.Launch();

            if (IsAssignable(sourceType, destinationType))
                return $"                {sourceName}";

            if (TryGetCollectionType(sourceType, out var sourceElementType, out var _) && TryGetCollectionType(destinationType, out var destinationElementType, out var postfix))
            {
                return $"{sourceName}.Select(x{nextListNameNumber} => {FormatMapping($"x{nextListNameNumber++}", sourceElementType, destinationElementType, nextListNameNumber, extensions)}){postfix}";
            }

            var sourceProperties = GetCombinedSourceProperties(sourceName, sourceType, extensions);
            var destinationProperties = GetPropertiesRecursively(destinationType);

            return
$@"            new {destinationType.ToDisplayString()}()
            {{
                {string.Join($",{Environment.NewLine}                ", destinationProperties.Select(dp => FormatPropertyMapping(sourceProperties[dp.Name], dp, nextListNameNumber, extensions)))}
            }}";
        }

        private Dictionary<string, (ITypeSymbol Type, string Source)> GetCombinedSourceProperties(string sourceName, ITypeSymbol sourceType, ITypeSymbol[] extensions)
        {
            var sourceProperties = GetPropertiesRecursively(sourceType).ToDictionary(p => p.Name, p => (p.Type , sourceName));
            for (var i = 0; i < extensions.Length; i++)
            {
                foreach (var property in GetPropertiesRecursively(extensions[i]))
                {
                    sourceProperties[property.Name] = (property.Type, $"m._extensionSource{i+1}");
                }
            }

            return sourceProperties;
        }

        private string FormatPropertyMapping((ITypeSymbol Type, string Source) sourceType, IPropertySymbol destinationProperty, int nextListNameNumber, ITypeSymbol[] extensions)
        {
            if (IsAssignable(sourceType.Type, destinationProperty.Type))
                return $"{destinationProperty.Name} = {sourceType.Source}.{destinationProperty.Name}";

            return $"{destinationProperty.Name} = {FormatMapping($"{sourceType.Source}.{destinationProperty.Name}", sourceType.Type, destinationProperty.Type, nextListNameNumber, extensions)}";
        }

        private string FormatExensionMapper(int extensionNumber, ITypeSymbol[] extensionTypes) =>
$@"public sealed class Extension{extensionNumber}
    {{
        private readonly {_sourceType.Name} _source;
{string.Join($"{Environment.NewLine}", extensionTypes.Select((e, i) => $"        private readonly {e.GetQualifiedName()} _extensionSource{i + 1};"))}
        
        public Extension{extensionNumber}({_sourceType.Name} source, {string.Join($", ", extensionTypes.Select((e, i) => $"{e.GetQualifiedName()} ext{i + 1}"))})
        {{
            _source = source;
{string.Join($"{Environment.NewLine}", extensionTypes.Select((e, i) => $"           _extensionSource{i + 1} = ext{i + 1};"))}
        }}

        {string.Join(Environment.NewLine, _destinationTypes.Select(d => FormatImplicitOperator($"{_sourceType.Name}_Mapper.Extension{extensionNumber} m", _sourceType, d, extensionTypes)))}
    }}";
    }
}
