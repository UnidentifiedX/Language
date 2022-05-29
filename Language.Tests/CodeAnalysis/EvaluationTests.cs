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

        [InlineData("(a represents 10) multiplied by a", 100)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}
