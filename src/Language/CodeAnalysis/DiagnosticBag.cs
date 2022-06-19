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

        public void Report(TextLocation location, string message)
        {
            var diagnostic = new Diagnostic(location, message); 
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type)
        {
            var message = $"The number {text} is not a valid {type}";
            Report(location, message);
        }

        public void ReportBadCharacter(TextLocation location, char current)
        {

            var message = $"Bad character input: '{current}'";
            Report(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            var message = $"Unterminated string literal";
            Report(location, message);
        }

        public void ReportUnexpectedToken(TextLocation location, SyntaxKind providedKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token <{providedKind}>, expected <{expectedKind}>";
            Report(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string operatorText, TypeSymbol operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'";
            Report(location, message);
        }

        public void ReportUndefinedBinaryOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'";
            Report(location, message);
        }

        internal void ReportParameterAlreadyDeclared(TextLocation location, string parameterName)
        {
            var message = $"A parameter with the name '{parameterName}' has already been declared";
            Report(location, message);
        }

        public void ReportUndefinedVariable(TextLocation location, string name)
        {
            var message = $"Variable '{name}' does not exist";
            Report(location, message);
        }

        public void ReportNotAVariable(TextLocation location, string name)
        {
            var message = $"'{name}' is not a variable";
            Report(location, message);
        }

        public void ReportUndefinedType(TextLocation location, string name)
        {
            var message = $"Type '{name}' does not exist";
            Report(location, message);
        }

        public void ReportCannotConvert(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert from type '{fromType}' to '{toType}'";
            Report(location, message);
        }

        internal void ReportCannotConvertImplicitly(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert implicitly from type '{fromType}' to '{toType}' (are you missing a cast?)";
            Report(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            var message = $"'{name}' has already been declared";
            Report(location, message);
        }

        internal void ReportCannotAssign(TextLocation location, string name)
        {
            var message = $"Variable '{name}' is a constant and cannot be assigned to";
            Report(location, message);
        }

        public void ReportUndefinedFunction(TextLocation location, string name)
        {
            var message = $"Function '{name}' does not exist";
            Report(location, message);
        }

        public void ReportNotAFunction(TextLocation location, string name)
        {
            var message = $"'{name}' is not a function";
            Report(location, message);
        }

        public void ReportWrongArgumentCount(TextLocation location, string name, int expectedCount, int actualCount)
        {
            var message = $"Function '{name}' expects {expectedCount} arguments, but received {actualCount}";
            Report(location, message);
        }

        public void ReportWrongArgumentType(TextLocation location, string name, TypeSymbol expectedType, TypeSymbol actualType)
        {
            var message = $"Parameter '{name}' expects a value of type '{expectedType}' but was provided a value of type '{actualType}'";
            Report(location, message);
        }

        internal void ReportInvalidBreakOrContinue(TextLocation location, string text)
        {
            var message = $"'{text}' can only be used within a loop";
            Report(location, message);
        }

        public void ReportAllPathsMustReturn(TextLocation location)
        {
            var message = $"Not all code paths return a value";
            Report(location, message);
        }

        public void ReportExpressionMustHaveValue(TextLocation location)
        {
            var message = $"Expression must have a value";
            Report(location, message);
        }

        public void ReportInvalidReturn(TextLocation location)
        {
            var message = $"The 'return' keyword can only be used within a function";
            Report(location, message);
        }

        public void ReportMissingReturnExpression(TextLocation location, TypeSymbol returnType)
        {
            var message = $"An expression of type '{returnType}' was expected";
            Report(location, message);
        }

        public void ReportInvalidReturnExpression(TextLocation location, string functionName)
        {
            var message = $"Function '{functionName}' does not have a return value and hence cannot be followed by an expression";
            Report(location, message);
        }
    }
}
