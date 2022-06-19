namespace Language.CodeAnalysis.Syntax
{
    public sealed class TypeClauseSyntax : SyntaxNode
    {
        public TypeClauseSyntax(SyntaxTree syntaxTree, SyntaxToken asToken, SyntaxToken identifier) 
            : base(syntaxTree)
        {
            AsToken = asToken;
            Identifier = identifier;
        }

        public override SyntaxKind Kind => SyntaxKind.TypeClause;
        public SyntaxToken AsToken { get; }
        public SyntaxToken Identifier { get; }
    }
}