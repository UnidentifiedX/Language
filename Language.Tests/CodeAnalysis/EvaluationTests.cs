using Language.CodeAnalysis;
using Language.CodeAnalysis.Symbols;
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
        [InlineData("~1", -2)]
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
        
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 2", 0)]
        [InlineData("1 & 0", 0)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("0 ^ 1", 1)]
        [InlineData("1 ^ 3", 2)]

        [InlineData("false | false", false)]
        [InlineData("false | true", true)]
        [InlineData("true | false", true)]
        [InlineData("true | true", true)]
        [InlineData("false & false", false)]
        [InlineData("false & true", false)]
        [InlineData("true & false", false)]
        [InlineData("true & true", true)]
        [InlineData("false ^ false", false)]
        [InlineData("true ^ false", true)]
        [InlineData("false ^ true", true)]
        [InlineData("true ^ true", false)]

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

        [InlineData("true and true", true)]
        [InlineData("false or false", false)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("not true", false)]
        [InlineData("not false", true)]

        [InlineData("\"test\"", "test")]
        [InlineData("\"te\"\"st\"", "te\"st")]
        [InlineData("\"test\" is equal to \"test\"", true)]
        [InlineData("\"test\" is not equal to \"test\"", false)]        
        [InlineData("\"test\" is equal to \"abc\"", false)]
        [InlineData("\"test\" is not equal to \"abc\"", true)]

        [InlineData("variable a represents 10", 10)]
        [InlineData(":variable a represents 10 (a multiplied by a).", 100)]
        [InlineData(":variable a represents 0 (a represents 10) multiplied by a.", 100)]
        [InlineData(":variable a represents 0 if a is equal to 0 a represents 10 a.", 10)]
        [InlineData(":variable a represents 0 if a is equal to 4 a represents 10 a.", 0)]        
        [InlineData(":variable a represents 0 if a is equal to 0 a represents 10 else a represents 5 a.", 10)]
        [InlineData(":variable a represents 0 if a is equal to 4 a represents 10 else a represents 5 a.", 5)]

        [InlineData(":variable i represents 10 variable result represents 0 while i is greater than 0: result represents result plus i i represents i minus 1. result.", 55)]
        [InlineData(":variable result represents 0 for i represents 1 to 10: result represents result plus i. result.", 55)]
        [InlineData(":variable a represents 10 for i represents 1 to (a represents a minus 1): . a.", 9)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            AssertValue(text, expectedValue);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
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
                'x' has already been declared
            ";

            AssertDiagnostics(text, diagnostics);
        }         
        
        [Fact]
        public void Evaluator_BlockStatement_NoInfiniteLoop()
        {
            var text = @"
                :
                [)][]
            ";

            var diagnostics = @"
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>
                Unexpected token <EndOfFileToken>, expected <PeriodToken>
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_NameExpression_Reports_Undefined()
        {
            var text = @"[x] multiplied by 10";

            var diagnostics = @"
                Variable 'x' does not exist
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_NoErrorForInsertedToken()
        {
            var text = @"1 plus []";

            var diagnostics = @"
                Unexpected token <EndOfFileToken>, expected <IdentifierToken>
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_Undefined()
        {
            var text = @"[x] represents 10";

            var diagnostics = @"
                Variable 'x' does not exist
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotAssign()
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
        public void Evaluator_AssignmentExpression_Reports_CannotConvert()
        {
            var text = @"
                :
                    variable x represents 10
                    x represents [true]
                .
            ";

            var diagnostics = @"
                Cannot convert from type 'boolean' to 'integer'
            ";

            AssertDiagnostics(text, diagnostics);
        }            
        
        [Fact]
        public void Evaluator_Variables_Can_Shadow_Functions()
        {
            var text = @"
                :
                    constant output represents 42
                    [output](""test"")
                .
            ";

            var diagnostics = @"
                Function 'output' does not exist
            ";

            AssertDiagnostics(text, diagnostics);
        }          
        
        [Fact]
        public void Evaluator_IfStatement_Reports_CannotConvert()
        {
            var text = @"
                :
                    variable x represents 0
                    if [10]
                        x represents 10
                .
            ";

            var diagnostics = @"
                Cannot convert from type 'integer' to 'boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }          
        
        [Fact]
        public void Evaluator_WhileStatement_Reports_CannotConvert()
        {
            var text = @"
                :
                    variable x represents 0
                    while [10]
                        x represents 10
                .
            ";

            var diagnostics = @"
                Cannot convert from type 'integer' to 'boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }              
        
        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
        {
            var text = @"
                :
                    variable result represents 0
                    for i represents [false] to 10
                        result represents result plus i
                .
            ";

            var diagnostics = @"
                Cannot convert from type 'boolean' to 'integer'
            ";

            AssertDiagnostics(text, diagnostics);
        }              
        
        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
        {
            var text = @"
                :
                    variable result represents 0
                    for i represents 1 to [true]
                        result represents result plus i
                .
            ";

            var diagnostics = @"
                Cannot convert from type 'boolean' to 'integer'
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_UnaryExpression_Reports_Undefined()
        {
            var text = @"[+]true";

            var diagnostics = @"
                Unary operator '+' is not defined for type 'boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }        
        
        [Fact]
        public void Evaluator_BinaryExpression_Reports_Undefined()
        {
            var text = @"10 [multiplied by] false";

            var diagnostics = @"
                Binary operator 'multiplied by' is not defined for types 'integer' and 'boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_InvokeFunctionArguments_NoInfiniteLoop()
        {
            var text = @"
                output(""Hi""[[represents]][)]
            ";

            var diagnostics = @"
                Unexpected token <RepresentsToken>, expected <CloseParenthesisToken>
                Unexpected token <RepresentsToken>, expected <IdentifierToken>
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_FunctionParameters_NoInfiniteLoop()
        {
            var text = @"
                function hi(name as string [[[represents]]][)]
                :
                    output(""Hi "" plus name plus ""!"")
                .[]
            ";

            var diagnostics = @"
                Unexpected token <RepresentsToken>, expected <CloseParenthesisToken>
                Unexpected token <RepresentsToken>, expected <ColonToken>
                Unexpected token <RepresentsToken>, expected <IdentifierToken>
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>
                Unexpected token <EndOfFileToken>, expected <PeriodToken>
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
