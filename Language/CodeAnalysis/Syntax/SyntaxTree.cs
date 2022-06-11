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
            Diagnostics = parser.Diagnostics.ToImmutableArray();
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

        public static ImmutableArray<SyntaxToken> ParseTokens(string text, bool removeWhitespace)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText, removeWhitespace);
        }              
        
        public static ImmutableArray<SyntaxToken> ParseTokens(string text, bool removeWhitespace, out ImmutableArray<Diagnostic> diagnostics)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText, removeWhitespace, out diagnostics);
        }        
        
        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text, bool removeWhitespace)
        {
            return ParseTokens(text, removeWhitespace, out _);
        }       
        
        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text, bool removeWhitespace, out ImmutableArray<Diagnostic> diagnostics)
        {
            IEnumerable<SyntaxToken> LexTokens(Lexer lexer)
            {
                while (true)
                {
                    var token = lexer.Lex();
                    if (token.Kind == SyntaxKind.EndOfFileToken) break;
                    if (removeWhitespace && token.Kind == SyntaxKind.WhitespaceToken) continue;

                    yield return token;
                }
            }

            var l = new Lexer(text);
            var result = LexTokens(l).ToImmutableArray();
            diagnostics = l.Diagnostics.ToImmutableArray();
            return result;
        }
    }
}
