using Language.CodeAnalysis.Symbols;
using System;

namespace Language.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}
