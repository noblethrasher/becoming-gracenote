namespace Gracenote
{
    class NotEqualExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&#8800;";
        }
    }

    class AlmostEqualExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&#8776;";
        }
    }

    class GreaterThanOrEqualExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&#8805;";
        }
    }

    class LessThanOrEqualExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&#8804;";
        }
    }
}
