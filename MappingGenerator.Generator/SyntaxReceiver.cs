using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace MappingGenerator.Generator
{
    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<(SyntaxNode Source, SyntaxNode DestinationInstance, SyntaxType Type)> PropertyAssignments { get; } = new List<(SyntaxNode Source, SyntaxNode DestinationInstance, SyntaxType Type)>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            //System.Diagnostics.Debugger.Launch();

            // Match the map method
            var source = GetMapSourceNode(syntaxNode);

            if (source != null)
            {
                //System.Diagnostics.Debugger.Launch();

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
                    PropertyAssignments.Add((source, property, SyntaxType.Default));
                }

                // Match "B b = X"
                if (syntaxNode.Parent is EqualsValueClauseSyntax
                    {
                        Parent: VariableDeclaratorSyntax
                        {
                            Parent: VariableDeclarationSyntax
                            {
                                Type: IdentifierNameSyntax
                                {

                                } type
                            }
                        }
                    })
                {
                    //System.Diagnostics.Debugger.Launch();
                    PropertyAssignments.Add((source, type, SyntaxType.Default));

                }

                // Match "a.SomeMethod(X)"
                if (syntaxNode.Parent is ArgumentSyntax
                    {
                    } argument)
                {
                    PropertyAssignments.Add((source, argument, SyntaxType.MethodArgument));
                }
            }
        }

        private SyntaxNode GetMapSourceNode(SyntaxNode syntaxNode)
        {
            return syntaxNode switch
            {
                InvocationExpressionSyntax
                {
                    ArgumentList: { Arguments: { Count: 0 } },
                    Expression: MemberAccessExpressionSyntax { Name: { Identifier: { ValueText: "Map" } }, Expression: IdentifierNameSyntax {} identifierNode }
                } => identifierNode,

                InvocationExpressionSyntax
                {
                    ArgumentList: { Arguments: { Count: 0 } },
                    Expression: MemberAccessExpressionSyntax { Name: { Identifier: { ValueText: "Map" } }, Expression: MemberAccessExpressionSyntax {} memberAccessNode }
                } => memberAccessNode,

                _ => null
            };
        }
    }

    enum SyntaxType
    {
        Default,
        MethodArgument
    }
}
