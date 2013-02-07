using System.Linq;
using System.Collections.Generic;

namespace Gracenote
{
    public sealed class TimeExpression : TerminalSpan
    {
        string hour, minute, period;

        public TimeExpression(IEnumerable<char> hour, IEnumerable<char> minute, IEnumerable<char> period)
        {
            this.hour = new string (hour.ToArray ());
            this.minute = new string (minute.ToArray ());
            this.period = new string (period.ToArray ());
        }

        public override string GenerateCode()
        {
            return "<span class=\"gn_time\">" + hour + ":" + minute + "<span class=\"gn_period\">" + period + "</span></span>";
        }
    }
}