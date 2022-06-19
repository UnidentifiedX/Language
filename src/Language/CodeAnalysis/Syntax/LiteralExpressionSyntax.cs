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
        public LiteralExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken literalToken) 
            : this(syntaxTree, literalToken, literalToken.Value)
        {

        }

        public LiteralExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken literalToken, object value)
            : base(syntaxTree)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public SyntaxToken LiteralToken { get; }
        public object Value { get; }
    }
}
