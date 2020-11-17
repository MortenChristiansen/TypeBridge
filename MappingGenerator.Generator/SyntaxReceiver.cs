using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace MappingGenerator.Generator
{
    class SyntaxReceiver : ISyntaxReceiver
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
                    PropertyAssignments.Add((source, property));
                }

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
                    PropertyAssignments.Add((source, type));

                }
            }
        }
    }
}
