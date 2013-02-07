namespace Gracenote
{
    using System;

    [Guard('_')]
    public sealed class Underscore : SpanToken
    {
        public Underscore(char c)
            : base (c)
        {
            
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c == '_')
            {
                parentStack.Pop ();
                return new DoubleUnderscore (c);
            }
            else
                return Start (ref index);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new UnderscoreExpression ();
        }
    }

    [Guard ('_')]
    public sealed class DoubleUnderscore : SpanToken
    {
        public DoubleUnderscore(char c)
            : base (c)
        {
            Add (c);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialUnderlinedExpression (index);
        }
    }
}