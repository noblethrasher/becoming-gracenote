namespace Gracenote
{
    [Guard('&')]
    public sealed class Ampersand : SpanToken
    {
        public Ampersand(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new AmpersandExpression ();
        }
    }
}