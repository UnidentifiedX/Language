using System;
using System.Collections.Generic;
namespace Language.CodeAnalysis
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.NotToken:
                    return 6;
                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ModuloToken:
                case SyntaxKind.MultiplicationToken:
                case SyntaxKind.DivisionToken:
                    return 5;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;
                case SyntaxKind.EqualityToken:
                case SyntaxKind.InequalityToken:
                    return 3;
                case SyntaxKind.AndToken:
                    return 2;
                case SyntaxKind.OrToken:
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
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                default:
                    return SyntaxKind.IdentifierToken;

            }
        }

        public static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return "plus";
                case SyntaxKind.MinusToken:
                    return "minus";
                case SyntaxKind.MultiplicationToken:
                    return "multiplied by";
                case SyntaxKind.DivisionToken:
                    return "divided by";
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";
                case SyntaxKind.NotToken:
                    return "not";
                case SyntaxKind.AndToken:
                    return "and";
                case SyntaxKind.OrToken:
                    return "or";
                case SyntaxKind.EqualityToken:
                    return "is equal to";
                case SyntaxKind.InequalityToken:
                    return "is not equal to";
                case SyntaxKind.RepresentsToken:
                    return "represents";
                case SyntaxKind.ModuloToken:
                    return "modulo";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.FalseKeyword:
                    return "false";
                default:
                    return null;
            }
        }
    }
}
