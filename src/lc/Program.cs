using Language.CodeAnalysis;
using Language.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Language
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.Error.WriteLine("usage lc <source-paths>");
                return;
            }

            if(args.Length > 1)
            {
                Console.WriteLine("error: only one path is currently supported");
            }

            var path = args.Single();
            var text = File.ReadAllText(path);
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<CodeAnalysis.Symbols.VariableSymbol, object>());

            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                    Console.WriteLine(result.Value);
            }
            else
            {
                Console.Error.WriteDiagnostics(result.Diagnostics, syntaxTree);
            }
        }
    }
}