using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Language.Classes.Parser;

namespace Language.Classes
{
    internal class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\0';

                return _text[_position];
            }
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
        {
            // <numbers>
            // + - * / ( )
            // <whitespace>

            if(_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFile, _position, "\0", null);

            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if(!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"The number {_text} is not a valid Int32");
                }

                return new SyntaxToken(SyntaxKind.Number, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.Whitespace, start, text, null);
            }

            if (Current == '+')
                return new SyntaxToken(SyntaxKind.Plus, _position++, "+", null);
            else if (Current == '-')
                return new SyntaxToken(SyntaxKind.Minus, _position++, "-", null);
            else if (Current == '*')
                return new SyntaxToken(SyntaxKind.Star, _position++, "*", null);
            else if (Current == '/')
                return new SyntaxToken(SyntaxKind.Slash, _position++, "/", null);
            else if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesis, _position++, "(", null);
            else if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesis, _position++, ")", null);

            _diagnostics.Add($"ERROR: Bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

    enum SyntaxKind
    {
        Number,
        Whitespace,
        Plus,
        Minus,
        Star,
        Slash,
        OpenParenthesis,
        CloseParenthesis,
        BadToken,
        EndOfFile,
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}
