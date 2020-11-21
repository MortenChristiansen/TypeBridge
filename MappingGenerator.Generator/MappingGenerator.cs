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

                if (formatter.HasMappings)
                {
                    var mapper = formatter.Format();
                    var extensions = MapExtensionFormatter.Format(m.Key);
                    var combined = NamespaceFormatter.Format(m.Key.GetNamespace(), mapper) + Environment.NewLine + Environment.NewLine + extensions;
                    context.AddSource($"{m.Key.Name}_Mapper.cs", combined);
                }
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

            //System.Diagnostics.Debugger.Launch();

            foreach ((var source, var destination, var type) in args)
            {
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

                // Normal method
                if (type == SyntaxType.MethodArgument && destination is ArgumentSyntax
                    {
                        Parent: ArgumentListSyntax
                        {
                            Parent: InvocationExpressionSyntax
                            {
                                Expression: MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax {} methodOwner, Name: IdentifierNameSyntax { Identifier: {} methodName } }
                            },
                            Arguments: {} methodArguments
                        }
                    } methodArgument)
                {
                    var parameter = GetParameter(semanticModel, methodOwner, methodName, methodArguments, null, methodArgument);
                    if (parameter == null)
                        continue;

                    destinationType = parameter.Type;
                    if (destinationType is null || foundTypes.Contains((sourceType, destinationType)))
                        continue;
                }

                // Constructor method
                if (type == SyntaxType.MethodArgument && destination is ArgumentSyntax
                    {
                        Parent: ArgumentListSyntax
                        {
                            Parent: ObjectCreationExpressionSyntax { Type: TypeSyntax {} constructedType },
                            Arguments: {} arguments
                        }
                    } constructorArgument)
                {
                    var parameter = GetConstructorParameter(semanticModel, constructedType, arguments, null, constructorArgument);
                    if (parameter == null)
                        continue;

                    destinationType = parameter.Type;
                    if (destinationType is null || foundTypes.Contains((sourceType, destinationType)))
                        continue;
                }

                // Generic method
                if (type == SyntaxType.MethodArgument && destination is ArgumentSyntax
                    {
                        Parent: ArgumentListSyntax
                        {
                            Parent: InvocationExpressionSyntax
                            {
                                Expression: MemberAccessExpressionSyntax
                                {
                                    Expression: IdentifierNameSyntax {} genericMethodOwner,
                                    Name: GenericNameSyntax { Identifier: SyntaxToken {} genericMethodName, TypeArgumentList: TypeArgumentListSyntax {} typeArguments }
                                }
                            },
                            Arguments: {} genericArguments
                        }
                    } genericArgument)
                {
                    var parameter = GetParameter(semanticModel, genericMethodOwner, genericMethodName, genericArguments, typeArguments, genericArgument);
                    if (parameter == null)
                        continue;

                    var potentiallyGenericDestinationType = parameter.Type;
                    if (potentiallyGenericDestinationType is null || foundTypes.Contains((sourceType, potentiallyGenericDestinationType)))
                        continue;

                    if (potentiallyGenericDestinationType.TypeKind == TypeKind.TypeParameter)
                    {
                        var methodSymbol = potentiallyGenericDestinationType.ContainingSymbol as IMethodSymbol;
                        var typeArgumentIndex = methodSymbol.TypeArguments.IndexOf(potentiallyGenericDestinationType);
                        if (typeArgumentIndex >= 0 && typeArguments.Arguments.Count > typeArgumentIndex)
                        {
                            var argument = GetNamedType(semanticModel, typeArguments.Arguments[typeArgumentIndex]);

                            if (argument != null &&!foundTypes.Contains((sourceType, potentiallyGenericDestinationType)))
                                destinationType = argument;
                        }
                    }
                    else
                    {
                        destinationType = potentiallyGenericDestinationType;
                    }
                }

                if (destinationType != null)
                {
                    foundTypes.Add((sourceType, destinationType));
                    yield return new MappingInfo { SourceType = sourceType, DestinationType = destinationType };
                }
            }
        }

        private IParameterSymbol GetParameter(
            SemanticModel semanticModel,
            IdentifierNameSyntax methodOwner,
            SyntaxToken methodName,
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            TypeArgumentListSyntax genericArguments,
            ArgumentSyntax argument
        ) =>
            semanticModel
                .GetTypeInfo(methodOwner)
                .Type?
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.Name == methodName.ValueText)
                .Select(m => (Match: HasMatchingParameters(m, arguments, argument, genericArguments, out var matchingParameter), matchingParameter))
                .FirstOrDefault(v => v.Match)
                .matchingParameter;

        private IParameterSymbol GetConstructorParameter(
            SemanticModel semanticModel,
            TypeSyntax constructedType,
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            TypeArgumentListSyntax genericArguments,
            ArgumentSyntax argument
        )
        {
            return GetNamedType(semanticModel, constructedType)?
                .Constructors
                .Select(m => (Match: HasMatchingParameters(m, arguments, argument, genericArguments, out var matchingParameter), matchingParameter))
                .FirstOrDefault(v => v.Match)
                .matchingParameter;
        }

        private static INamedTypeSymbol GetNamedType(SemanticModel semanticModel, TypeSyntax type) =>
            semanticModel.GetSymbolInfo(type).Symbol as INamedTypeSymbol;

        // For normal methods
        private bool HasMatchingParameters(IMethodSymbol method, SeparatedSyntaxList<ArgumentSyntax> argumentList, ArgumentSyntax argument, TypeArgumentListSyntax typeArguments, out IParameterSymbol matchingParameter)
        {
            matchingParameter = null;

            if (method.Parameters.Length != argumentList.Count)
                return false;

            if (typeArguments != null && typeArguments.Arguments.Count != method.TypeArguments.Length)
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
