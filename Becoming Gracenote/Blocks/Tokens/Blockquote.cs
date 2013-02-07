namespace Gracenote
{
    public sealed class BlockQuote : LineToken
    {
        public BlockQuote(string line)
            : base (line)
        {
            
        }

        public override SyntaxNode StartNode(ref int i)
        {
            return new PartialBlockQuote (this);
        }

    }
}