namespace Gracenote
{
    public sealed class BlankLineExpression : TerminalLine
    {
        readonly Blank blank;

        public BlankLineExpression(Blank blank)
        {
            this.blank = blank;
        }

        public int Lines
        {
            get
            {
                return blank.NumberOfBlankLines;
            }
        }

        public override string GenerateCode(uint n = 0)
        {
            return "";
        }
    }
}