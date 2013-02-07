namespace Gracenote
{
    public sealed class EmptyImage : SpanToken
    {
        public EmptyImage() { }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new EmptyImageExpression ();
        }
    }
}