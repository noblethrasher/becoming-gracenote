namespace Gracenote
{
    using System;

    public sealed class WordToken : SpanToken
    {
        public WordToken(char c)
            : base (c)
        {
            if (!char.IsLetter (c))
                throw new ArgumentException ();
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c.IsLetter)
            {
                Add (c);
                return this;
            }
            else
            {
                return Start (ref index);
            }
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new WordExpression (GetString());
        }
    }
}