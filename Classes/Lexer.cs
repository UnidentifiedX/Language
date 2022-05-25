using System;
using System.Collections.Generic;

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

        private char Current => Peek(0);
        private string LookAhead(int offsetStart, int offsetEnd)
        {
            char[] charArr = new char[offsetEnd - offsetStart + 1];
            for(int i = offsetStart; i <= offsetEnd; i++)
            {
                charArr[i - 1] = Peek(i);
            }

            return new string(charArr);
        }

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        { 
            if(_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFile, _position, "\0", null);

            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"The number {_text} is not a valid Int32");
                }

                return new SyntaxToken(SyntaxKind.Number, start, text, value);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.Plus, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.Minus, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.Star, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.Slash, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesis, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesis, _position++, ")", null);
                case 'a':
                    if (LookAhead(1, 2) == "nd")
                        return new SyntaxToken(SyntaxKind.And, _position += 3, "and", null);
                    break;
                case 'o':
                    if (LookAhead(1, 1) == "r")
                        return new SyntaxToken(SyntaxKind.Or, _position += 2, "or", null);
                    break;
                case 'i':
                    if (LookAhead(1, 10) == "s equal to")
                        return new SyntaxToken(SyntaxKind.Equality, _position += 11, "is equal to", null);
                    else if (LookAhead(1, 14) == "s not equal to")
                        return new SyntaxToken(SyntaxKind.Inequality, _position += 15, "is not equal to", null);
                    break;
                case '!':
                        return new SyntaxToken(SyntaxKind.Bang, _position++, "!", null);
                default:
                    if (char.IsLetter(Current))
                    {
                        var start = _position;

                        while (char.IsLetter(Current)) Next();

                        var length = _position - start;
                        var text = _text.Substring(start, length);
                        var kind = SyntaxFacts.GetKeywordKind(text);
                        return new SyntaxToken(kind, start, text, null);
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

                    break;
            }

            _diagnostics.Add($"ERROR: Bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}
