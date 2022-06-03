using Language.CodeAnalysis.Syntax;
using Language.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Language.CodeAnalysis
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText text)
        {            
            var parser = new Parser(text);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            Text = text;
            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }        
        
        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text, bool removeWhitespace)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText, removeWhitespace);
        }        
        
        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text, bool removeWhitespace)
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
