namespace Gracenote
{
    [Guard(';')]
    public sealed class Semicolon : SpanToken
    {
        public Semicolon(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericPunctuationExpression (chars[0]);
        }
    }
}