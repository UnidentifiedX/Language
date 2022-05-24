using Language.Classes.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Language.Classes.Parser;

namespace Language.Classes
{
    internal class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        public int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is LiteralExpressionSyntax n)
            {
                return (int)n.LiteralToken.Value;
            }

            if (node is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);

                if (u.OperatorToken.Kind == SyntaxKind.Plus)
                    return operand;
                else if (u.OperatorToken.Kind == SyntaxKind.Minus)
                    return -operand;
                else
                    throw new Exception($"Unexpected unary operator {u.OperatorToken.Kind}");
            }

            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.Plus)
                    return left + right;
                else if (b.OperatorToken.Kind == SyntaxKind.Minus)
                    return left - right;
                else if (b.OperatorToken.Kind == SyntaxKind.Star)
                    return left * right;
                else if (b.OperatorToken.Kind == SyntaxKind.Slash)
                    return left / right;
                    
                else
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }

            if (node is ParenthesizedExpressionSyntax p) 
                return EvaluateExpression(p.Expression);

            throw new Exception($"Undexpected node {node.Kind}");
        }
    }
}
