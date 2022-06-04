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
        GreaterOrEqualsToken,
        LessOrEqualsToken,
        GreaterToken,
        LessToken,
        InequalityToken,
        NameExpression,
        RepresentsToken,
        ModuloToken,
        OpenBraceToken,
        CloseBraceToken,

        // Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,

        AssignmentExpression,
        PositiveToken,
        NegativeToken,

        // Nodes
        CompilationUnit,
        ElseClause,

        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,

        // Keywords
        ConstantKeyword,
        VariableKeyword,
        TrueKeyword,
        FalseKeyword,
        ElseKeyword,
        IfKeyword,
        WhileKeyword
    }
}
