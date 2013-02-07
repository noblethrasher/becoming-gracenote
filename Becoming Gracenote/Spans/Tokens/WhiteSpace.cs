namespace Gracenote
{
    using System;
    
    public sealed class WhiteSpace : SpanToken
    {
        public WhiteSpace(char c) : base(c)
        {
            if (!char.IsWhiteSpace (c))
                throw new ArgumentException ();
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c.IsWhiteSpace)
            {
                Add (c);
                return this;
            }
            else
            {
                return Start (ref index);
            }
        }

        public override int WhitesSpaceLength
        {
            get
            {
                return chars.Count;
            }
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialWhitespaceExpression (chars);
        }
        
        public override string ToString()
        {
            return base.ToString ().Replace("\r", "\\r").Replace("\n", "\\n") + "(" + WhitesSpaceLength + ")";
        }
    }
}