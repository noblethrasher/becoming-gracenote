using System;
using System.Collections.Generic;

namespace Gracenote
{
    class TimeToken : SpanToken
    {
        List<char> hour = new List<char> (2);
        List<char> minute = new List<char> (2);
        List<char> period = new List<char> ();

        int state = 0;

        Func<Indexical<CharEx>, SpanToken>[] actions;

        bool done = false;


        public TimeToken()
        {
            actions = new Func<Indexical<CharEx>, SpanToken>[3];

            actions[0] = ScanHour;
            actions[1] = ScanMinute;
            actions[2] = ScanPeriod;
        }
        

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            return done ? Start(ref index) : actions[state] (index);   
        }

        public SpanToken ScanHour(Indexical<CharEx> index)
        {
            if (index.Value != ':')
                hour.Add (index.Value);                
            else
                state = 1;

            return this;                
        }

        public SpanToken ScanMinute(Indexical<CharEx> index)
        {
            if (index.Value.IsDigit)
            {
                minute.Add (index.Value);
                return this;
            }
            else
            {
                state = 2;

                return actions[state] (index);
            }
        }

        public SpanToken ScanPeriod(Indexical<CharEx> index)
        {
            if (index.Value != '\\')
                period.Add (index.Value);                
            else
                done = true;

            return this;
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new TimeExpression (hour, minute, period);
        }
    }
}
