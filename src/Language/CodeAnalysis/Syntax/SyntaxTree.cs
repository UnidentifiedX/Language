using Language.CodeAnalysis.Syntax;
using Language.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Language.CodeAnalysis
{
    public sealed class SyntaxTree
    {
        private delegate void ParseHandler(SyntaxTree syntaxTree, bool removeWhiteSpace, out CompilationUnitSyntax root, out ImmutableArray<Diagnostic> diagnostics);

        private SyntaxTree(SourceText text, bool removeWhitespace, ParseHandler handler)
        {
            Text = text;
            handler(this, removeWhitespace, out var root, out var diagnostics);

            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }


        public static SyntaxTree Load(string fileName)
        {
            var text = File.ReadAllText(fileName);
            var sourceText = SourceText.From(text, fileName);

            return Parse(sourceText);
        }

        private static void Parse(SyntaxTree syntaxTree, bool removeWhitespace, out CompilationUnitSyntax root, out ImmutableArray<Diagnostic> diagnostics)
        {
            var parser = new Parser(syntaxTree);
            root = parser.ParseCompilationUnit();
            diagnostics = parser.Diagnostics.ToImmutableArray();
        }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }        
        
        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text, removeWhitespace: false, Parse);
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
            var tokens = new List<SyntaxToken>();

            void ParseTokens(SyntaxTree st, bool removeWhitespace, out CompilationUnitSyntax root, out ImmutableArray<Diagnostic> d)
            {
                root = null;

                var l = new Lexer(st);
                while (true)
                {
                    var token = l.Lex();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                    {
                        root = new CompilationUnitSyntax(st, ImmutableArray<MemberSyntax>.Empty, token);
                        break;
                    }
                    if (removeWhitespace && token.Kind == SyntaxKind.WhitespaceToken) continue;

                    tokens.Add(token);
                }

                d = l.Diagnostics.ToImmutableArray();
            }

            var syntaxTree = new SyntaxTree(text, removeWhitespace, ParseTokens);
            diagnostics = syntaxTree.Diagnostics.ToImmutableArray();
            return tokens.ToImmutableArray();
        }
    }
}
