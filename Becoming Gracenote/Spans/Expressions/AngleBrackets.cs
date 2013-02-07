namespace Gracenote
{
    public sealed class LeftAngleBracketExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&lt;";
        }
    }

    public sealed class RightAngleBracketExpression : TerminalSpan
    {

        public override string GenerateCode()
        {
            return "&gt;";
        }
    }
}