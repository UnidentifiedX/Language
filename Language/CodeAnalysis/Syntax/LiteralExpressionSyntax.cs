using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Language.CodeAnalysis.Parser;

namespace Language.CodeAnalysis.Syntax
{
    sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalToken) 
            : this(literalToken, literalToken.Value)
        {

        }

        public LiteralExpressionSyntax(SyntaxToken literalToken, object value)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public SyntaxToken LiteralToken { get; }
        public object Value { get; }
    }
}
