namespace Gracenote
{
    using System;

    [Guard('"')]
    public sealed class DoubleQuote : SpanToken
    {
        public DoubleQuote(char c)
            : base (c)
        {
            
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialInlineQuoteExpression (index);
        }
    }
}