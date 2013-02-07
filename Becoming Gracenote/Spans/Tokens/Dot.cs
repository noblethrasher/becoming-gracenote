namespace Gracenote
{
    [Guard('.')]
    public sealed class Dot : SpanToken
    {
        public Dot(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericSymbolExpression (chars[0]);
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c == '.')
            {
                parentStack.Pop ();
                return new DoubleDot (c);
            }
            else
                return base.Scan (ref index);
        }
    }

    [Guard ('.')]
    public sealed class DoubleDot : SpanToken
    {
        public DoubleDot(char c)
            : base (c)
        {
            Add (c);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericSymbolExpression (chars.ToArray());
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c == '.')
            {
                parentStack.Pop ();
                return new TripleDot ('.');
            }
            else
                return base.Scan (ref index);
        }
    }

    [Guard ('.')]
    public sealed class TripleDot : SpanToken
    {
        public TripleDot(char c)
            : base (c)
        {
            Add (c);
            Add (c);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new ElipsisExpression (index);
        }
    }
}