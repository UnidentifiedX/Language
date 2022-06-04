using Language.CodeAnalysis;
using System;
using System.Collections.Generic;
using Xunit;

namespace Language.Tests.CodeAnalysis
{
    public class EvaluationTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("14 plus 12", 26)]
        [InlineData("12 minus 3", 9)]
        [InlineData("4 multiplied by 2", 8)]
        [InlineData("9 divided by 3", 3)]
        [InlineData("9 modulo 2", 1)]
        [InlineData("(10)", 10)]

        [InlineData("4 is greater than 3", true)]
        [InlineData("4 is greater than 5", false)]
        [InlineData("4 is greater than or equal to 4", true)]
        [InlineData("5 is greater than or equal to 4", true)]
        [InlineData("4 is greater than or equal to 5", false)]        
        
        [InlineData("3 is less than 4", true)]
        [InlineData("5 is less than 4", false)]
        [InlineData("4 is less than or equal to 4", true)]
        [InlineData("4 is less than or equal to 5", true)]
        [InlineData("5 is less than or equal to 4", false)]

        [InlineData("12 is equal to 3", false)]
        [InlineData("3 is equal to 3", true)]
        [InlineData("12 is not equal to 3", true)]
        [InlineData("3 is not equal to 3", false)]
        [InlineData("false is equal to false", true)]
        [InlineData("true is equal to false", false)]        
        [InlineData("false is not equal to false", false)]
        [InlineData("true is not equal to false", true)]

        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("not true", false)]
        [InlineData("not false", true)]

        [InlineData(":variable a represents 0 (a represents 10) multiplied by a .", 100)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            AssertValue(text, expectedValue);
        }

        [Fact]
        public void Evaluator_Variable_Declaration_Reports_Redeclaration()
        {
            var text = @"
                :
                    variable x represents 10
                    variable y represents 100
                    :
                        variable x represents 10
                    .
                    variable [x] represents 5
                .
            ";

            var diagnostics = @"
                Variable 'x' has already been declared
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_Name_Reports_Undefined()
        {
            var text = @"[x] multiplied by 10";

            var diagnostics = @"
                Variable 'x' does not exist
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_Assigned_Reports_Undefined()
        {
            var text = @"[x] represents 10";

            var diagnostics = @"
                Variable 'x' does not exist
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_Assigned_Reports_CannotAssign()
        {
            var text = @"
                :
                    constant x represents 10
                    x [represents] 0
                .
            ";

            var diagnostics = @"
                Variable 'x' is a constant and cannot be assigned to
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_Assigned_Reports_CannotConvert()
        {
            var text = @"
                :
                    variable x represents 10
                    x represents [true]
                .
            ";

            var diagnostics = @"
                Cannot convert from type 'System.Boolean' to 'System.Int32'
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_Unary_Reports_Undefined()
        {
            var text = @"[+]true";

            var diagnostics = @"
                Unary operator '+' is not defined for type 'System.Boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_Binary_Reports_Undefined()
        {
            var text = @"10 [multiplied by] false";

            var diagnostics = @"
                Binary operator 'multiplied by' is not defined for types 'System.Int32' and 'System.Boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private static void AssertValue(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        private static void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
                throw new Exception("ERROR: Must mark as many spans as there are exepcted diagnostics");

            Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

            for(int i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;                      
                Assert.Equal(expectedMessage, actualMessage);
                
                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;
                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}
