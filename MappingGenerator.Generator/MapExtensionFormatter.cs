using Microsoft.CodeAnalysis;

namespace MappingGenerator.Generator
{
    static class MapExtensionFormatter
    {
        public static string Format(ITypeSymbol type) =>
$@"static class {type.Name}_MappingExtensions
    {{
        public static {type.GetQualifiedName()}_Mapper Map(this {type.GetQualifiedName()} t) =>
            new {type.GetQualifiedName()}_Mapper(t);
    }}";
    }
}
