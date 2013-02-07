namespace Gracenote
{
    [Guard(',')]
    public sealed class Comma : SpanToken
    {
        public Comma(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new CommaExpression ();
        }
    }
}