using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dotnet.Analyzers.Utility;

internal static class CodeGeneratorUtilities
{
    public static LocalDeclarationStatementSyntax IntroduceLocalVariableStatement(
        string parameterName,
        ExpressionSyntax replacementNode)
    {
        var equalsToReplacementNode = EqualsValueClause(replacementNode);

        var oneItemVariableDeclaration = VariableDeclaration(
            ParseTypeName("var"),
            SingletonSeparatedList(
                VariableDeclarator(Identifier(parameterName))
                    .WithInitializer(equalsToReplacementNode)
            )
        );

        return LocalDeclarationStatement(oneItemVariableDeclaration);
    }
}