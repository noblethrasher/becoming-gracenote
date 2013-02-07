namespace Gracenote
{
    [Guard('/')]
    public sealed class Forwardslash : SpanToken
    {
        public Forwardslash(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericPunctuationExpression (chars[0]);
        }
    }
}