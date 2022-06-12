﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language.CodeAnalysis;
using Language.CodeAnalysis.Binding;
using Language.CodeAnalysis.Symbols;
using Language.CodeAnalysis.Syntax;
using Language.CodeAnalysis.Text;

namespace Language
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var showTree = false;
            var showProgram = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();

            Compilation previous = null;

            while(true)
            {
                if (textBuilder.Length == 0)
                    Console.Write("> ");
                else
                    Console.Write("· ");

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if(textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        break;
                    }
                    else if (input == "/showtree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Now showing parse trees" : "No longer showing parse trees");
                        continue;
                    }                    
                    else if (input == "/showprogram")
                    {
                        showProgram = !showProgram;
                        Console.WriteLine(showProgram ? "Now showing bound tree" : "No longer showing bound tree");
                        continue;
                    }
                    else if (input == "/cls")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if(input == "/reset")
                    {
                        previous = null;
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compilation = previous == null ? new Compilation(syntaxTree) : previous.ContinueWith(syntaxTree);


                if (showTree)
                    syntaxTree.Root.WriteTo(Console.Out);       
                
                if (showProgram)
                    compilation.EmitTree(Console.Out);

                var result = compilation.Evaluate(variables);

                if (!result.Diagnostics.Any())
                {
                    if(result.Value != null)
                    {
                        Console.WriteLine(result.Value);
                        previous = compilation;
                    }
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;

                        var character = diagnostic.Span.Start - line.Start + 1;

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diagnostic.Span);
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

                textBuilder.Clear();
            }
        }
    }
}
