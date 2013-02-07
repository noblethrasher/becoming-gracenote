namespace Gracenote
{
    using System;

    [Guard('\'')]
    public sealed class SingleQuote : SpanToken
    {
        public SingleQuote(char c)
            : base (c)
        {
            
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            if (index.Value == '\'')
            {
                parentStack.Pop ();
                return new DoublePrime ();
            }
            else
                return base.Scan (ref index);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new ApostropheExpression (index);
        }
    }

    public sealed class DoublePrime : SpanToken
    {
        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new SingleQuoteExpression ();
        }
    }
}