using Language.CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Language.CodeAnalysis
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();  
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text, bool removeWhitespace)
        {
            var lexer = new Lexer(text);
            while(true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EndOfFileToken) break;
                if (removeWhitespace && token.Kind == SyntaxKind.WhitespaceToken) continue;

                yield return token;
            }
        }
    }
}
