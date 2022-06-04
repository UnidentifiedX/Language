﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace Language.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary <string, VariableSymbol> _variables = new();

 

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public BoundScope Parent { get; }

        public bool TryDeclare(VariableSymbol variable)
        {
            if(_variables.ContainsKey(variable.Name)) 
                return false;

            _variables.Add(variable.Name, variable);
            return true;
        }        
        
        public bool TryLookup(string name, out VariableSymbol variable)
        {
            if(_variables.TryGetValue(name, out variable))
                return true;

           if(Parent == null)
                return false;

           return Parent.TryLookup(name, out variable);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return _variables.Values.ToImmutableArray();
        }
    }
}