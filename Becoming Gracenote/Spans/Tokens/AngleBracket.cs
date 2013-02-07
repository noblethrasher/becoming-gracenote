namespace Gracenote
{
    public abstract class AngleBracket : SpanToken
    {
        public AngleBracket(char c)
            : base (c)
        {

        }
    }

    [Guard('<')]
    public sealed class LeftAngleBracket : SpanToken
    {
        public LeftAngleBracket(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new LeftAngleBracketExpression ();
        }
    }

    [Guard ('>')]
    public sealed class RightAngleBracket : SpanToken
    {
        public RightAngleBracket(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new RightAngleBracketExpression ();
        }
    }
}