using Language.CodeAnalysis.Syntax;

namespace Language.CodeAnalysis
{
    internal class ContinueStatementSyntax : StatementSyntax
    {
        public ContinueStatementSyntax(SyntaxTree syntaxTree, SyntaxToken keyword)
            :base(syntaxTree)
        {
            Keyword = keyword;
        }

        public override SyntaxKind Kind => SyntaxKind.ContinueStatement;
        public SyntaxToken Keyword { get; }

    }
}