namespace Gracenote
{
    public sealed class Header : LineToken
    {
        public Header(string line)
            : base (line)
        {

        }

        public override SyntaxNode StartNode(ref int i)
        {
            return new PartialHeader (this);
        }

        internal SyntaxNode MakeHeaderExpression()
        {
            var c = line[0];

            switch (c)
            {
                case '=':
                    return new PartialHeader.HeaderExpression.H1 (line);
                case '-':
                    return new PartialHeader.HeaderExpression.H2 (line);
            }
        }
    }    
}