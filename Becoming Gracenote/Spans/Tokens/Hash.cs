namespace Gracenote
{
    [Guard('#')]
    public sealed class Hash : SpanToken
    {
        public Hash(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericPunctuationExpression (chars[0]);
        }
    }
}