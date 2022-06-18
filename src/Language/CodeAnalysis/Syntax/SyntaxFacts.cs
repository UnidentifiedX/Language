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
                case SyntaxKind.PositiveToken:
                case SyntaxKind.NegativeToken:
                case SyntaxKind.NotToken:
                case SyntaxKind.BitwiseNegationToken:
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
                case SyntaxKind.LessToken:
                case SyntaxKind.GreaterToken:
                case SyntaxKind.GreaterOrEqualsToken:
                case SyntaxKind.LessOrEqualsToken:
                    return 3;
                case SyntaxKind.AndToken:
                case SyntaxKind.BitwiseAndToken:
                    return 2;
                case SyntaxKind.OrToken:
                case SyntaxKind.BitwiseOrToken:
                case SyntaxKind.BitwiseXorToken:
                    return 1;
                default:
                    return 0;
            }
        }

        internal static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "if":
                    return SyntaxKind.IfKeyword;                
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                case "for":
                    return SyntaxKind.ForKeyword;                
                case "to":
                    return SyntaxKind.ToKeyword;
                case "constant":
                    return SyntaxKind.ConstantKeyword;
                case "variable":
                    return SyntaxKind.VariableKeyword;                
                case "while":
                    return SyntaxKind.WhileKeyword;                
                case "function":
                    return SyntaxKind.FunctionKeyword;
                case "return":
                    return SyntaxKind.ReturnKeyword;
                case "break":
                    return SyntaxKind.BreakKeyword;
                case "continue":
                    return SyntaxKind.ContinueKeyword;
                default:
                    return SyntaxKind.IdentifierToken;

            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach(var kind in kinds)
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PositiveToken:
                    return "+";
                case SyntaxKind.NegativeToken:
                    return "-";
                case SyntaxKind.BitwiseNegationToken:
                    return "~";
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
                case SyntaxKind.GreaterOrEqualsToken:
                    return "is greater than or equal to";                
                case SyntaxKind.LessOrEqualsToken:
                    return "is less than or equal to";                
                case SyntaxKind.GreaterToken:
                    return "is greater than";                
                case SyntaxKind.LessToken:
                    return "is less than";
                case SyntaxKind.InequalityToken:
                    return "is not equal to";
                case SyntaxKind.RepresentsToken:
                    return "represents";
                case SyntaxKind.ModuloToken:
                    return "modulo";
                case SyntaxKind.ColonToken:
                    return ":";
                case SyntaxKind.PeriodToken:
                    return ".";                
                case SyntaxKind.CommaToken:
                    return ",";
                case SyntaxKind.AsToken:
                    return "as";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.FalseKeyword:
                    return "false";                   
                case SyntaxKind.ForKeyword:
                    return "for";                   
                case SyntaxKind.ToKeyword:
                    return "to";                
                case SyntaxKind.ConstantKeyword:
                    return "constant";
                case SyntaxKind.VariableKeyword:
                    return "variable";                
                case SyntaxKind.WhileKeyword:
                    return "while";                
                case SyntaxKind.FunctionKeyword:
                    return "function";                            
                case SyntaxKind.ReturnKeyword:
                    return "return";                
                case SyntaxKind.BreakKeyword:
                    return "break";                
                case SyntaxKind.ContinueKeyword:
                    return "continue";
                case SyntaxKind.BitwiseAndToken:
                    return "&";                
                case SyntaxKind.BitwiseOrToken:
                    return "|";                
                case SyntaxKind.BitwiseXorToken:
                    return "^";
                default:
                    return null;
            }
        }
    }
}
