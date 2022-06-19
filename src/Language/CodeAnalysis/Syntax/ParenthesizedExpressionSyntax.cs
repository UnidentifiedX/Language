using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Language.CodeAnalysis.Parser;

namespace Language.CodeAnalysis.Syntax
{
    sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken openParenthesis, ExpressionSyntax expression, SyntaxToken closeParenthesis)
            : base(syntaxTree)
        {
            OpenParenthesisToken = openParenthesis;
            Expression = expression;
            CloseParenthesisToken = closeParenthesis;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }
    }
}
