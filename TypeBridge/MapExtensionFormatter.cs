using Microsoft.CodeAnalysis;

namespace TypeBridge
{
    static class MapExtensionFormatter
    {
        public static string Format(ITypeSymbol type) =>
$@"static class {type.Name}_MappingExtensions
    {{
        public static {type.GetQualifiedName()}_Mapper Map(this {type.ToDisplayString()} t) =>
            new {type.GetQualifiedName()}_Mapper(t);
    }}";
    }
}
