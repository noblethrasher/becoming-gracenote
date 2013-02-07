namespace Gracenote
{
    public sealed class CommaExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return ",";
        }
    }
}