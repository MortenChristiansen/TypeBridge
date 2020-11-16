using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingGenerator.Generator
{
    [Generator]
    public class MappingGenerator : ISourceGenerator
    {
        // TODO: If the two types are the same, just return the original (or do not emit a mapping method)
        // TODO: Map all properties - Can we use a recursive Map call to map all the complex members?
        // TODO: IEnumerables should be mapped to IEnumerabes. Can we use a recursive Map call to map all the collection elements?
        // TODO: If no mapping is possible, then just not emit mapping. It may be difficult to figure out why a mapping is not available.
        // TODO: We can skip the generic argument when we can resolve the type from the context (what the result is assigned to)
        // TODO: Make sure we can use the mapping function like this "things.Select(Map.To)"
        // TODO: Make sure we support mapping from tuples. This will be a central use case for Dapper I think
        // TODO: Use full type names when there are name clashes.

        // Should we require all fields in the destination to be mapped and allow missing fields via an object (anonymous or not)
        // Map.To<BigThing>(smallThing, new { MissingThing = thing });

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
            {
                throw new InvalidOperationException("Wrong syntax receiver");
            }

            var argumentsToValidate = syntaxReceiver.PropertyAssignments;
            var mappingInfos = GetMappingInfo(context.Compilation, argumentsToValidate);
            var mappingInfoByType = mappingInfos.GroupBy(m => m.SourceType); // TODO: More robust grouping
            foreach (var m in mappingInfoByType)
            {
                var formatter = new TypeMapperFormatter(m.Key);
                foreach (var mappingInfo in m)
                    formatter.AddDestinationType(mappingInfo.DestinationType);

                var mapper = formatter.Format();
                var extensions = MapExtensionFormatter.Format(m.Key);
                var combined = NamespaceFormatter.Format(m.Key.GetNamespace(), mapper, extensions);
                context.AddSource($"{m.Key.Name}_Mapper.cs", combined);
                //System.Diagnostics.Debugger.Launch();
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private IEnumerable<MappingInfo> GetMappingInfo(Compilation compilation, List<(SyntaxNode Source, SyntaxNode DestinationInstance)> args)
        {
            var foundTypes = new HashSet<(ITypeSymbol Source, ITypeSymbol Destination)>();

            foreach ((var source, var destination) in args)
            {
                //System.Diagnostics.Debugger.Launch();
                var semanticModel = compilation.GetSemanticModel(source.SyntaxTree);

                var sourceType = semanticModel.GetTypeInfo(source).Type;
                if (sourceType is null)
                    continue;

                var destinationType = semanticModel.GetTypeInfo(destination).Type;
                if (destinationType is null || foundTypes.Contains((sourceType, destinationType)))
                    continue;

                foundTypes.Add((sourceType, destinationType));
                yield return new MappingInfo { SourceType = sourceType, DestinationType = destinationType };
            }
        }
    }

    public class MappingInfo
    {
        public ITypeSymbol SourceType { get; set; }
        public ITypeSymbol DestinationType { get; set; }

        public static readonly MappingInfo DefaultMapping = new MappingInfo { SourceType = null, DestinationType = null };
    }

    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<(SyntaxNode Source, SyntaxNode DestinationInstance)> PropertyAssignments { get; } = new List<(SyntaxNode Source, SyntaxNode DestinationInstance)>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InvocationExpressionSyntax
            {
                ArgumentList:
                {
                    Arguments:
                    {
                        Count: 0
                    }
                }
                ,
                Expression: MemberAccessExpressionSyntax
                {
                    Name:
                    {
                        Identifier:
                        {
                            ValueText: "Map"
                        }
                    },
                    Expression: IdentifierNameSyntax
                    {

                    } source
                }
            })
            {
                // Match "a.B = X"
                if (syntaxNode.Parent is AssignmentExpressionSyntax
                {
                    Left: MemberAccessExpressionSyntax
                    {
                        Expression: IdentifierNameSyntax
                        {
                        },
                        Name: IdentifierNameSyntax
                        {

                        } property // The property "B"
                    }
                })
                {
                    //System.Diagnostics.Debugger.Launch();
                    PropertyAssignments.Add((source, property));
                }
            }
        }
    }
}
