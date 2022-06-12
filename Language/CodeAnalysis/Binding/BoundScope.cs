using Language.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Language.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, Symbol> _symbols;

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public BoundScope Parent { get; }

        public bool TryDeclareVariable(VariableSymbol variable) => TryDeclareSymbol(variable);
        public bool TryDeclareFunction(FunctionSymbol function) => TryDeclareSymbol(function);

        private bool TryDeclareSymbol<T>(T symbol)
            where T : Symbol
        {
            if (_symbols == null)
                _symbols = new Dictionary<string, Symbol>();
            else if (_symbols.ContainsKey(symbol.Name))
                return false;

            _symbols.Add(symbol.Name, symbol);
            return true;
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable) => TryLookupSymbol(name, out variable);
        public bool TryLookupFunction(string name, out FunctionSymbol function) => TryLookupSymbol(name, out function);

        private bool TryLookupSymbol<T>(string name, out T symbol)
            where T : Symbol
        {
            symbol = null;

            if(_symbols != null && _symbols.TryGetValue(name, out var declaredSymbol))
            {
                if(declaredSymbol is T matchingSymbol)
                {
                    symbol = matchingSymbol;
                    return true;
                }

                return false;
            }

            if (Parent == null)
                return false;

            return Parent.TryLookupSymbol(name, out symbol);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables() => GetDeclaredSymbols<VariableSymbol>();

        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions() => GetDeclaredSymbols<FunctionSymbol>();

        private ImmutableArray<T> GetDeclaredSymbols<T>()
            where T : Symbol
        {
            if(_symbols == null)
                return ImmutableArray<T>.Empty;

            return _symbols.Values.OfType<T>().ToImmutableArray();
        }
    }
}
