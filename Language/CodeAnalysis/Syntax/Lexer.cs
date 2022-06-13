using Language.CodeAnalysis.Symbols;
using Language.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Language.CodeAnalysis
{
    internal class Lexer
    {
        private readonly SourceText _text;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;

        private int _start;
        private SyntaxKind _kind;
        private object _value;

        public Lexer(SourceText text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

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

        public SyntaxToken Lex()
        { 
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EndOfFileToken;
                    break;
                case '+':
                    _kind = SyntaxKind.PositiveToken;
                    _position++;
                    break;
                case '-':
                    _kind = SyntaxKind.NegativeToken;
                    _position++;
                    break;
                case '(':
                    _kind = SyntaxKind.OpenParenthesisToken;
                    _position++;
                    break;
                case ')':
                    _kind = SyntaxKind.CloseParenthesisToken;
                    _position++;
                    break;
                case ':':
                    _kind = SyntaxKind.ColonToken;
                    _position++;
                    break;
                case '.':
                    _kind = SyntaxKind.PeriodToken;
                    _position++;
                    break;                
                case ',':
                    _kind = SyntaxKind.CommaToken;
                    _position++;
                    break;
                case '~':
                    _kind = SyntaxKind.BitwiseNegationToken;
                    _position++;
                    break;
                case '&':
                    _kind = SyntaxKind.BitwiseAndToken;
                    _position++;
                    break;                
                case '|':
                    _kind = SyntaxKind.BitwiseOrToken;
                    _position++;
                    break;                
                case '^':
                    _kind = SyntaxKind.BitwiseXorToken;
                    _position++;
                    break;
                case 'a':
                    if (LookAhead(1, 2) == "nd")
                    {
                        _kind = SyntaxKind.AndToken;
                        _position += 3;
                        break;
                    }
                    else goto default;
                case 'd':
                    if(LookAhead(1, 9) == "ivided by")
                    {
                        _kind = SyntaxKind.DivisionToken;
                        _position += 10;
                        break;
                    }
                    else goto default;
                case 'i':
                    if (LookAhead(1, 10) == "s equal to")
                    {
                        _kind = SyntaxKind.EqualityToken;
                        _position += 11;
                        break;
                    }
                    else if (LookAhead(1, 26) == "s greater than or equal to")
                    {
                        _kind = SyntaxKind.GreaterOrEqualsToken;
                        _position += 27;
                        break;
                    }
                    else if (LookAhead(1, 23) == "s less than or equal to")
                    {
                        _kind = SyntaxKind.LessOrEqualsToken;
                        _position += 24;
                        break;
                    }
                    else if(LookAhead(1, 14) == "s greater than")
                    {
                        _kind = SyntaxKind.GreaterToken;
                        _position += 15;
                        break;
                    }                    
                    else if(LookAhead(1, 11) == "s less than")
                    {
                        _kind = SyntaxKind.LessToken;
                        _position += 12;
                        break;
                    }
                    else if (LookAhead(1, 14) == "s not equal to")
                    {
                        _kind = SyntaxKind.InequalityToken;
                        _position += 15;
                        break;
                    }
                    else goto default;
                case 'm':
                    if(LookAhead(1, 4) == "inus")
                    {
                        _kind = SyntaxKind.MinusToken;
                        _position += 5;
                        break;
                    } 
                    else if(LookAhead(1, 12) == "ultiplied by")
                    {
                        _kind = SyntaxKind.MultiplicationToken;
                        _position += 13;
                        break;
                    }
                    else if(LookAhead(1, 5) == "odulo")
                    {
                        _kind = SyntaxKind.ModuloToken;
                        _position += 6;
                        break;
                    }
                    else goto default;
                case 'n':
                    if (LookAhead(1, 2) == "ot")
                    {
                        _kind = SyntaxKind.NotToken;
                        _position += 3;
                        break;
                    }
                    else goto default;
                case 'o':
                    if (LookAhead(1, 1) == "r")
                    {
                        _kind = SyntaxKind.OrToken;
                        _position += 2;
                        break;
                    }
                    else goto default;
                case 'p':
                    if (LookAhead(1, 3) == "lus")
                    {
                        _kind = SyntaxKind.PlusToken;
                        _position += 4;
                        break;
                    }
                    else goto default;
                case 'r':
                    if(LookAhead(1, 9) == "epresents")
                    {
                        _kind = SyntaxKind.RepresentsToken;
                        _position += 10;
                        break;
                    }
                    else goto default;
                case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9':
                    ReadNumberToken();
                    break;
                case '"':
                    ReadString();
                    break;
                case ' ': case '\t': case '\n': case '\r': 
                    ReadWhiteSpaceToken();
                    break;
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeywordToken();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpaceToken();
                    }
                    else
                    {
                        _diagnostics.ReportBadCharacter(_position, Current);
                        _position++;
                    }
                    break;
            }

            var length = _position - _start;
            var text = SyntaxFacts.GetText(_kind);
            if (text == null)
                text = _text.ToString(_start, length);

            return new SyntaxToken(_kind, _start, text, _value);
        }

        private void ReadString()
        {
            // Skip current quote
            _position++;
            var sb = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(_start, 1);
                        _diagnostics.ReportUnterminatedString(span);
                        done = true;
                        break;
                    case '"':
                        if(LookAhead(1, 1) == "\"")
                        {
                            sb.Append('"');
                            _position += 2;
                        }
                        else
                        {
                            _position++;
                            done = true;
                        }
                        break;
                    default:
                        sb.Append(Current);
                        _position++;
                        break;
                }
            }

            _kind = SyntaxKind.StringToken;
            _value = sb.ToString();
        }

        private void ReadWhiteSpaceToken()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);
            if (!int.TryParse(text, out var value))
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, TypeSymbol.Int);

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }

        private void ReadIdentifierOrKeywordToken()
        {
            while (char.IsLetter(Current)) _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
        }
    }
}
