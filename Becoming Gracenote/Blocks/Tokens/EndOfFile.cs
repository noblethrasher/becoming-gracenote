namespace Gracenote
{
    public sealed class EndOfFileToken : LineToken
    {
        public EndOfFileToken() : base( ((char)26).ToString())
        {
            
        }

        public override SyntaxNode StartNode(ref int i)
        {
            return new EndOfFileExpression ();
        }

        sealed class EndOfFileExpression : SyntaxNode
        {
            public override SyntaxNode Parse(LineToken token, ref int i)
            {
                return this;
            }
        }
    }
}