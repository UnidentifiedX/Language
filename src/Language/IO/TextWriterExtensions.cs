using Language.CodeAnalysis;
using Language.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language.IO
{
    public static class TextWriterExtensions
    {
        public static void WriteDiagnostics(this TextWriter writer, IEnumerable<Diagnostic> diagnostics, SyntaxTree syntaxTree)
        {
            foreach (var diagnostic in diagnostics
                .OrderBy(d => d.Location.Text.FileName)
                .ThenBy(d => d.Location.Span.Start)
                .ThenBy(d => d.Location.Span.Length))
            {
                var fileName = diagnostic.Location.FileName;
                var startLine = diagnostic.Location.StartLine + 1;
                var startCharacter = diagnostic.Location.StartCharacter + 1;
                var endLine = diagnostic.Location.EndLine + 1;
                var endCharacter = diagnostic.Location.EndCharacter + 1;

                var span = diagnostic.Location.Span;
                var lineIndex = syntaxTree.Text.GetLineIndex(span.Start);
                var line = syntaxTree.Text.Lines[lineIndex];

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): ");
                Console.WriteLine(diagnostic);
                Console.ResetColor();

                var prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
                var suffixSpan = TextSpan.FromBounds(span.End, line.End);

                var prefix = syntaxTree.Text.ToString(prefixSpan);
                var error = syntaxTree.Text.ToString(span);
                var suffix = syntaxTree.Text.ToString(suffixSpan);

                Console.Write("    ");
                Console.Write(prefix);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(error);
                Console.ResetColor();

                Console.Write(suffix);
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
