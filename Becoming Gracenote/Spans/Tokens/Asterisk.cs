namespace Gracenote
{
    using System;

    [Guard('*')]
    public sealed class Asterisk : SpanToken
    {
        public Asterisk(char c)
            : base (c)
        {
            
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c == '*')
            {
                parentStack.Pop ();
                return new DoubleAsterisk (c);
            }
            else
                return Start (ref index);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialItalic (index);
        }
    }

    [Guard('*')]
    public sealed class DoubleAsterisk : SpanToken
    {
        public DoubleAsterisk(char c) : base(c)
        {
            Add (c);
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c == '*')
            {
                parentStack.Pop ();
                return new TripleAsterisk (c);
            }
            else
                return Start (ref index);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialBoldExpression (index);
        }
    }

    [Guard ('*')]
    public sealed class TripleAsterisk : SpanToken
    {
        public TripleAsterisk(char c)
            : base (c)
        {
            Add (c);
            Add (c);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialBoldItalicExpression (index);
        }
    }
}