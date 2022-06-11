using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Language.CodeAnalysis.Symbols
{
    internal static class BuitinFunctions
    {
        public static FunctionSymbol Print = new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
        public static FunctionSymbol Input = new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);

        internal static IEnumerable<FunctionSymbol> GetAll()
            => typeof(BuitinFunctions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(FunctionSymbol))
                .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}
