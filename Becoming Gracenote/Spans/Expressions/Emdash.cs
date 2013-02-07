namespace Gracenote
{
    public sealed class EmdashExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&#8212;";
        }
    }
}