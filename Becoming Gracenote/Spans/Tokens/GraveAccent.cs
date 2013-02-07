namespace Gracenote
{
    using System;

    [Guard('`')]
    public sealed class GraveAccent : SpanToken
    {
        public GraveAccent(char c)
            : base (c)
        {
            
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c.IsGraveAccent)
            {
                parentStack.Pop ();
                return new DoubleGraveAccent (c);
            }
            else
                return Start (ref index);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialBackTickQuotedExpression (index);
        }
    }

    [Guard ('`')]
    public sealed class DoubleGraveAccent : SpanToken
    {
        public DoubleGraveAccent(char c)
            : base (c)
        {
            Add (c);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialDoubleBackTickQuotedExpression (index);
        }
    }
}