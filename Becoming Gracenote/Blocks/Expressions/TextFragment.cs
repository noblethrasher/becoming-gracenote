using System;

namespace Gracenote
{
    public sealed class TextFragmentExpression : TerminalLine
    {
        readonly TextFragmentToken fragment;

        public TextFragmentExpression(TextFragmentToken fragment)
        {
            this.fragment = fragment;
        }

        public override string GenerateCode(uint n = 0)
        {
            return '\t'.Repeat (n) + "<span class=\"_f_\">" + new ParsedTextRun(((string)fragment)).GenerateCode() + "</span>";
        }       
    }
}