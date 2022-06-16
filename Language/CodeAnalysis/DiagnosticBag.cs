using Language.CodeAnalysis.Symbols;
using Language.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Language.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }        

        public void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message); 
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type)
        {
            var message = $"The number {text} is not a valid {type}";
            Report(span, message);
        }

        public void ReportBadCharacter(int position, char current)
        {
            var span = new TextSpan(position, 1);
            var message = $"Bad character input: '{current}'";
            Report(span, message);
        }

        public void ReportUnterminatedString(TextSpan span)
        {
            var message = $"Unterminated string literal";
            Report(span, message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind providedKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token <{providedKind}>, expected <{expectedKind}>";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'";
            Report(span, message);
        }

        internal void ReportParameterAlreadyDeclared(TextSpan span, string parameterName)
        {
            var message = $"A parameter with the name '{parameterName}' has already been declared";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"Variable '{name}' does not exist";
            Report(span, message);
        }

        public void ReportUndefinedType(TextSpan span, string name)
        {
            var message = $"Type '{name}' does not exist";
            Report(span, message);
        }

        internal void ReportFunctionsAreUnsupported(TextSpan span)
        {
            var message = $"Functions with return values are unsupported";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert from type '{fromType}' to '{toType}'";
            Report(span, message);
        }

        internal void ReportCannotConvertImplicitly(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert implicitly from type '{fromType}' to '{toType}' (are you missing a cast?)";
            Report(span, message);
        }

        public void ReportSymbolAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"'{name}' has already been declared";
            Report(span, message);
        }

        internal void ReportCannotAssign(TextSpan span, string name)
        {
            var message = $"Variable '{name}' is a constant and cannot be assigned to";
            Report(span, message);
        }

        public void ReportUndefinedFunction(TextSpan span, string name)
        {
            var message = $"Function '{name}' does not exist";
            Report(span, message);
        }

        public void ReportWrongArgumentCount(TextSpan span, string name, int expectedCount, int actualCount)
        {
            var message = $"Function '{name}' expects {expectedCount} arguments, but received {actualCount}";
            Report(span, message);
        }

        public void ReportWrongArgumentType(TextSpan span, string name, TypeSymbol expectedType, TypeSymbol actualType)
        {
            var message = $"Parameter '{name}' expects a value of type '{expectedType}' but was provided a value of type '{actualType}'";
            Report(span, message);
        }

        internal void ReportInvalidBreakOrContinue(TextSpan span, string text)
        {
            var message = $"'{text}' can ony be used within a loop";
            Report(span, message);
        }

        public void ReportExpressionMustHaveValue(TextSpan span)
        {
            var message = $"Expression must have a value";
            Report(span, message);
        }


    }
}
