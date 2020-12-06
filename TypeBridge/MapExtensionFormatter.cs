using Microsoft.CodeAnalysis;

namespace TypeBridge
{
    static class MapExtensionFormatter
    {
        public static string Format(ITypeSymbol type) =>
$@"static class {type.GetUngenericizedName()}_MappingExtensions
    {{
        public static {type.GetUngenericizedName()}_Mapper Map(this {type.ToDisplayString()} t) =>
            new {type.GetUngenericizedName()}_Mapper(t);
    }}";
    }
}
