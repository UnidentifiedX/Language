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
        ColonToken,
        PeriodToken,
        BitwiseNegationToken,
        BitwiseAndToken,
        BitwiseXorToken,

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
        ForStatement,

        // Keywords
        ConstantKeyword,
        VariableKeyword,
        TrueKeyword,
        FalseKeyword,
        ElseKeyword,
        IfKeyword,
        WhileKeyword,
        ForKeyword,
        ToKeyword,
        BitwiseOrToken,
        StringToken,
        CallExpression,
        CommaToken,
        AsToken,
        TypeClause,
        GlobalStatement,
        FunctionDeclaration,
        Parameter,
        FunctionKeyword,
        ContinueKeyword,
        BreakKeyword,
        ContinueStatement,
        BreakStatement,
        ReturnKeyword,
        ReturnStatement
    }
}
