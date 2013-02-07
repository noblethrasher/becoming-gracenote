namespace Gracenote
{
    public class Digits : SpanToken
    {
        public Digits(char c) : base(c)
        {
            
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            var c = index.Value;
            
            if (c.IsDigit)
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
            return new NumberExpression (GetString ());
        }
    }
}