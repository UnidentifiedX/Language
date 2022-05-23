using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language.Classes
{
    internal class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();

                if (token.Kind != SyntaxKind.Whitespace && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFile);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];
            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public abstract class SyntaxNode
        {
            public abstract SyntaxKind Kind { get; }
            public abstract IEnumerable<SyntaxNode> GetChildren();
        }

        public abstract class ExpressionSyntax : SyntaxNode
        {

        }

        public sealed class NumberExpressionSyntax : ExpressionSyntax
        {
            public NumberExpressionSyntax(SyntaxToken numberToken)
            {
                NumberToken = numberToken;
            }

            public override SyntaxKind Kind => SyntaxKind.NumberExpression;
            public SyntaxToken NumberToken { get; }

            public override IEnumerable<SyntaxNode> GetChildren()
            {
                yield return NumberToken;
            }
        }

        public sealed class BinaryExpressionSyntax : ExpressionSyntax
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

        public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
        {
            public ParenthesizedExpressionSyntax(SyntaxToken openParenthesis, ExpressionSyntax expression, SyntaxToken closeParenthesis)
            {
                OpenParenthesis = openParenthesis;
                Expression = expression;
                CloseParenthesis = closeParenthesis;
            }
            public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
            public SyntaxToken OpenParenthesis { get; }
            public ExpressionSyntax Expression { get; }
            public SyntaxToken CloseParenthesis { get; }

            public override IEnumerable<SyntaxNode> GetChildren()
            {
                yield return OpenParenthesis;
                yield return Expression;
                yield return CloseParenthesis;
            }
        }

        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var endOfFile = Match(SyntaxKind.EndOfFile);

            return new SyntaxTree(_diagnostics, expression, endOfFile);
        }

        public ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (Current.Kind == SyntaxKind.Plus
                || Current.Kind == SyntaxKind.Minus)
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        public ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.Star
                || Current.Kind == SyntaxKind.Slash)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        public ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesis)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesis);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = Match(SyntaxKind.Number);
            return new NumberExpressionSyntax(numberToken);
        }

        public sealed class SyntaxTree
        {
            public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
            {
                Diagnostics = diagnostics.ToArray();
                Root = root;
                EndOfFileToken = endOfFileToken;
            }

            public IReadOnlyList<string> Diagnostics { get; }
            public ExpressionSyntax Root { get; }
            public SyntaxToken EndOfFileToken { get; }

            public static SyntaxTree Parse(string text)
            {
                var parser = new Parser(text);
                return parser.Parse();
            }
        }
    }
}
