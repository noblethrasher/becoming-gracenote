using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gracenote
{
    sealed class TrademarkExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&trade;";
        }
    }

    sealed class RegisteredTrademarkExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&reg;";
        }
    }

    sealed class CopyrightExpression : TerminalSpan
    {
        public override string GenerateCode()
        {
            return "&copy;";
        }
    }
}
