using Language.CodeAnalysis.Text;
using Xunit;

namespace Language.Tests.CodeAnalysis.Text
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text, int exprectedLineCount)
        {
            var sourceText = SourceText.From(text);
            Assert.Equal(exprectedLineCount, sourceText.Lines.Length);
        }
    }
}
