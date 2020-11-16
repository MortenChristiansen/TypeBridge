using Microsoft.CodeAnalysis;

namespace MappingGenerator.Generator
{
    static class MapExtensionFormatter
    {
        public static string Format(ITypeSymbol type) =>
$@"static class {type.Name}_MappingExtensions
    {{
        public static {type.Name}_Mapper Map(this {type.Name} t) =>
            new {type.Name}_Mapper(t);
    }}";
    }
}
