using Microsoft.CodeAnalysis;

namespace MappingGenerator.Generator
{
    class MappingInfo
    {
        public ITypeSymbol SourceType { get; set; }
        public ITypeSymbol DestinationType { get; set; }

        public static readonly MappingInfo DefaultMapping = new MappingInfo { SourceType = null, DestinationType = null };
    }
}
