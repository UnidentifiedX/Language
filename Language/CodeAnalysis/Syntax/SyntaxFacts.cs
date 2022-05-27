using System;
using System.Collections.Generic;
namespace Language.CodeAnalysis
{
    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                case SyntaxKind.Bang:
                    return 6;
                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.Percentage:
                case SyntaxKind.Star:
                case SyntaxKind.Slash:
                    return 5;
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 4;
                case SyntaxKind.Equality:
                case SyntaxKind.Inequality:
                    return 3;
                case SyntaxKind.And:
                    return 2;
                case SyntaxKind.Or:
                    return 1;
                default:
                    return 0;
            }
        }

        internal static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true":
                    return SyntaxKind.True;
                case "false":
                    return SyntaxKind.False;
                default:
                    return SyntaxKind.Identifier;

            }
        }
    }
}
