using Microsoft.CodeAnalysis;

namespace TypeBridge
{
    class MappingInfo
    {
        public ITypeSymbol SourceType { get; set; }
        public ITypeSymbol DestinationType { get; set; }
        public ITypeSymbol[] Extensions { get; set; }

        public static readonly MappingInfo DefaultMapping = new MappingInfo { SourceType = null, DestinationType = null, Extensions = new ITypeSymbol[0] };
    }
}
