namespace Gracenote
{
    public sealed class HorizontalRuleExpression : TerminalLine
    {
        readonly HorizontalRule hr;
        
        public HorizontalRuleExpression(HorizontalRule hr)
        {
            this.hr = hr;
        }

        public override string GenerateCode(uint n = 0)
        {
            return "<hr />";
        }
    }
}