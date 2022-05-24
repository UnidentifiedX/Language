using System.Collections.Generic;
using static Language.Classes.Parser;

namespace Language.Classes.Syntax
{
    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            Right = right;
            OperatorToken = operatorToken;
        }

        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }
        public SyntaxToken OperatorToken { get; }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}
