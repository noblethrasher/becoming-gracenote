namespace Gracenote
{
    public sealed class ElipsisExpression : TerminalSpan
    {
        public ElipsisExpression(Indexical<SpanToken> index)
        {

        }

        public override string GenerateCode()
        {
            return "&hellip;";
        }
    }
}