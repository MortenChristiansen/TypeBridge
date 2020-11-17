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

        private IEnumerable<MappingInfo> GetMappingInfo(Compilation compilation, List<(SyntaxNode Source, SyntaxNode DestinationInstance, SyntaxType Type)> args)
        {
            var foundTypes = new HashSet<(ITypeSymbol Source, ITypeSymbol Destination)>();

            foreach ((var source, var destination, var type) in args)
            {
                //System.Diagnostics.Debugger.Launch();
                var semanticModel = compilation.GetSemanticModel(source.SyntaxTree);

                var sourceType = semanticModel.GetTypeInfo(source).Type;
                if (sourceType is null)
                    continue;

                ITypeSymbol destinationType = null;
                if (type == SyntaxType.Default)
                {
                    destinationType = semanticModel.GetTypeInfo(destination).Type;
                    if (destinationType is null || foundTypes.Contains((sourceType, destinationType)))
                        continue;
                }
                if (type == SyntaxType.MethodArgument && destination is ArgumentSyntax
                    {
                        Parent: ArgumentListSyntax
                        {
                            Parent: InvocationExpressionSyntax
                            {
                                Expression: MemberAccessExpressionSyntax
                                {
                                    Expression: IdentifierNameSyntax
                                    {

                                    } methodOwner,
                                    Name: IdentifierNameSyntax
                                    {
                                        Identifier:
                                        {
                                            
                                        } methodName
                                    } method
                                }
                            },
                            Arguments:
                            {
                                
                            } arguments
                        } argumentList
                    } argument)
                {

                    var methodOwnerType = semanticModel.GetTypeInfo(methodOwner).Type;
                    var methodType = semanticModel.GetTypeInfo(method).Type;
                    var parameter = methodOwnerType.GetMembers().OfType<IMethodSymbol>().Where(m => m.Name == methodName.ValueText).Select(m => (Match: HasMatchingParameters(m, arguments, argument, out var matchingParameter), matchingParameter)).FirstOrDefault(v => v.Match).matchingParameter;
                    if (parameter == null)
                        continue;

                    //System.Diagnostics.Debugger.Launch();

                    destinationType = parameter.Type;
                    if (destinationType is null || foundTypes.Contains((sourceType, destinationType)))
                        continue;
                }

                foundTypes.Add((sourceType, destinationType));
                yield return new MappingInfo { SourceType = sourceType, DestinationType = destinationType };
            }
        }

        private bool HasMatchingParameters(IMethodSymbol method, SeparatedSyntaxList<ArgumentSyntax> argumentList, ArgumentSyntax argument, out IParameterSymbol matchingParameter)
        {
            matchingParameter = null;

            if (method.Parameters.Length != argumentList.Count)
                return false;

            //System.Diagnostics.Debugger.Launch();

            for (int i = 0; i < argumentList.Count; i++)
            {
                if (argumentList[i] == argument)
                    matchingParameter = method.Parameters[i];
            }

            // TODO: More robustly identify the correct method if possible
            return true;
        }
    }
}
