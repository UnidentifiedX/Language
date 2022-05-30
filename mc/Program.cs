﻿using System;
using System.Collections.Generic;
using System.Linq;
using Language.CodeAnalysis;
using Language.CodeAnalysis.Binding;
using Language.CodeAnalysis.Syntax;

namespace Language
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            bool showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();

            while(true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) return;

                if(line == "/showtree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Now showing parse trees" : "No longer showing parse trees");
                    continue;
                }
                else if (line == "/cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }
                
                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    var text = syntaxTree.Text;

                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - text.Lines[lineIndex].Start + 1;

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefix = line.Substring(0, diagnostic.Span.Start);
                        var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                        var suffix = line.Substring(diagnostic.Span.End);

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
    }
}
