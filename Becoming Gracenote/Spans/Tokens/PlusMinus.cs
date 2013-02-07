using System;
namespace Gracenote
{
    public sealed class PlusMinus : SpanToken
    {
        public PlusMinus(char plus, char slash, char minus) : base(plus)
        {
            if (plus != '+' || slash != '/' || minus != '-')
                throw new ArgumentException ();

            Add (slash);
            Add (minus);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PlusMinusExpression ();
        }
    }
}