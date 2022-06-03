namespace Language.CodeAnalysis
{
    public enum SyntaxKind
    {
        // Tokens
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        MultiplicationToken,
        DivisionToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        IdentifierToken,
        NotToken,
        AndToken,
        OrToken,
        EqualityToken,
        InequalityToken,
        NameExpression,
        RepresentsToken,
        ModuloToken,

        // Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        TrueKeyword,
        FalseKeyword,
        AssignmentExpression,
        PositiveToken,
        NegativeToken,

        // Noted
        CompilationUnit
    }
}
