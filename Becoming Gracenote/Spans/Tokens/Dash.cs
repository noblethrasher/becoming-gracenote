namespace Gracenote
{
    using System;

    [Guard('-')]
    public sealed class Dash : SpanToken
    {
        public Dash(char c)
            : base (c)
        {
            
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c == '-')
            {
                parentStack.Pop ();
                return new DoubleDash (c);
            }
            else
                return Start (ref index);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new DashExpression ();
            //return new GenericSymbolExpression (chars[0]);
        }
    }

    [Guard ('-')]
    public sealed class DoubleDash : SpanToken
    {
        public DoubleDash(char c)
            : base (c)
        {
            Add ('-');
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new EmdashExpression ();
        }
    }
}