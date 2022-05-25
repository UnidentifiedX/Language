namespace Language.Classes
{
    enum SyntaxKind
    {
        Number,
        Whitespace,
        Plus,
        Minus,
        Star,
        Slash,
        OpenParenthesis,
        CloseParenthesis,
        BadToken,
        EndOfFile,
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        True,
        False,
        IdentifierToken,
        Bang,
        And,
        Or,
        Equality,
        Inequality
    }
}
