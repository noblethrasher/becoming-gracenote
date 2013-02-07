namespace Gracenote
{
    [Guard('{')]
    public sealed class LeftCurlyBrace : SpanToken
    {
        public LeftCurlyBrace(char c) : base (c) { }
        
        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return null;
        }
    }

    [Guard ('}')]
    public sealed class RightCurlyBrace : SpanToken
    {
        public RightCurlyBrace(char c) : base (c) { }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return null;
        }
    }
}