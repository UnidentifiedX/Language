using System;

namespace Language.CodeAnalysis.Binding
{
    internal sealed class  BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeKind Kind => BoundNodeKind.Variable;
        public override Type Type => Variable.Type;
        public VariableSymbol Variable { get; }
    }
}
