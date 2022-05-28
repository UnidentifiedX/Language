namespace Language.CodeAnalysis
{
    public enum SyntaxKind
    {
        // Tokens
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,

        // Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        TrueKeyword,
        FalseKeyword,
        IdentifierToken,
        BangToken,
        AndToken,
        OrToken,
        EqualityToken,
        InequalityToken,
        NameExpression,
        AssignmentExpression,
        EqualsToken,
        PercentageToken
    }
}
