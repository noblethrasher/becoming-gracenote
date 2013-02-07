namespace Gracenote
{
    using System;

    [Guard ('^')]
    public sealed class Caret : SpanToken
    {
        public Caret(char c)
            : base (c)
        {

        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c.IsCaret)
                return new DoubleCaret (c);
            else
                return Start (ref index);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialSuperscriptExpression (index);
        }
    }

    [Guard ('^')]
    public sealed class DoubleCaret : SpanToken
    {
        public DoubleCaret(char c)
            : base (c)
        {
            Add (c);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialSubscriptExpression (index);
        }
    }
}