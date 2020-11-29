using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace TypeBridge.Generator
{
    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<(SyntaxNode Source, SyntaxNode DestinationInstance, ReceivedSyntaxType Type)> PropertyAssignments { get; } = new List<(SyntaxNode Source, SyntaxNode DestinationInstance, ReceivedSyntaxType Type)>();
        public List<(SyntaxNode Source, List<ArgumentSyntax> Extensions)> Extensions { get; } =
            new List<(SyntaxNode Source, List<ArgumentSyntax> Extensions)>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Match the map method
            var source = GetMapSourceNode(syntaxNode);

            if (source != null)
            {
                //System.Diagnostics.Debugger.Launch();

                AddPropertyAssignment(syntaxNode, source); // Normal Map()
                AddPropertyAssignment(syntaxNode.Parent.Parent, source); // Map().Extend(...)

                var extension = GetExtensions(source, syntaxNode);
                if (extension != default)
                {
                    //System.Diagnostics.Debugger.Launch();
                    Extensions.Add(extension);
                }
            }
        }

        private void AddPropertyAssignment(SyntaxNode syntaxNode, SyntaxNode source)
        {
            // Match "a.B = X"
            if (syntaxNode.Parent is AssignmentExpressionSyntax { Left: MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { }, Name: IdentifierNameSyntax { } property } })
            {
                PropertyAssignments.Add((source, property, ReceivedSyntaxType.Default));
                return;
            }

            // Match "_a = X"
            if (syntaxNode.Parent is AssignmentExpressionSyntax { Left: IdentifierNameSyntax { } member })
            {
                PropertyAssignments.Add((source, member, ReceivedSyntaxType.Default));
                return;
            }

            // Match "B b = X"
            if (syntaxNode.Parent is EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Type: IdentifierNameSyntax { } type } } })
            {
                PropertyAssignments.Add((source, type, ReceivedSyntaxType.Default));
                return;

            }

            // Match "B<T> b = X"
            if (syntaxNode.Parent is EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Type: GenericNameSyntax { } genericType } } })
            {
                PropertyAssignments.Add((source, genericType, ReceivedSyntaxType.Default));
                return;

            }

            // Match "a.SomeMethod(X)"
            if (syntaxNode.Parent is ArgumentSyntax { } argument)
            {
                PropertyAssignments.Add((source, argument, ReceivedSyntaxType.MethodArgument));
                return;
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

        private (SyntaxNode Source, List<ArgumentSyntax> Arguments) GetExtensions(SyntaxNode source, SyntaxNode syntaxNode)
        {
            return syntaxNode.Parent.Parent switch
            {
                InvocationExpressionSyntax
                {
                    ArgumentList: { Arguments: { Count: > 0 } arguments },
                    Expression: MemberAccessExpressionSyntax { Name: IdentifierNameSyntax { Identifier: { ValueText: "Extend" } } }
                } => (source, arguments.ToList()),

                InvocationExpressionSyntax
                {
                    ArgumentList: { Arguments: { Count: > 0 } arguments },
                    Expression: MemberAccessExpressionSyntax { Name: { Identifier: { ValueText: "Extend" } } }
                } => (source, arguments.ToList()),

                _ => default
            };
        }
    }
}
