using System;

namespace Gracenote
{
    public sealed class PlusMinusExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&#177;";
        }
    }
}
