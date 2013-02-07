namespace Gracenote
{
    public sealed class TextFragmentToken : LineToken
    {
        public TextFragmentToken(string line)
            : base (line)
        {

        }

        public override SyntaxNode StartNode(ref int i)
        {
            return new TextFragmentExpression (this);
        }
    }
}