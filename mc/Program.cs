using Language.CodeAnalysis.Binding;
using Language.CodeAnalysis.Syntax;

namespace Language
{
    internal static class Program
    {
        private static void Main()
        {
            var repl = new LanguageRepl();
            repl.Run();
        }
    }
}