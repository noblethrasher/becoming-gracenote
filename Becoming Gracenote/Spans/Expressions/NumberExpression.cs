using System;
using System.Collections.Generic;

namespace Gracenote
{
    public sealed class NumberExpression : NonTerminalSpan
    {
        readonly string number;

        static readonly Dictionary<string, Func<string, bool>> test = new Dictionary<string, Func<string, bool>> ()
        {
            { "st", s => s.ToUpper() == "ST"},
            { "nd", s => s.ToUpper() == "ND"},
            { "rd", s => s.ToUpper() == "RD"},
            { "th", s => s.ToUpper() == "TH"}
        };

        public NumberExpression(string number)
        {
            this.number = number;

            if (Parsed.WordCountAccumulator != null)
                Parsed.WordCountAccumulator (number);
        }

        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var token = index.Value;

            if (token is EndOfline)
            {
                Pop ();
                return new CardinalNumberExpression (number);
            }

            if (token is WordToken)
            {
                var s = token.GetString ();

                if (TestOrdinal (s))
                {
                    Pop ();
                    return new OrdinalNumberExpression (number, s);
                }

                if (s.ToUpper () == "X")
                {
                    var next = index + 1;

                    if (next.IsValid && next.Value is Digits)
                    {
                        return new DimesionExpression (number);
                    }
                }

                if (s == "DEG")
                    return new DegreeExpression (number);
            }

            Pop ();

            parentStack.Push (new CardinalNumberExpression (number));

            return token.GetStartNode (ref index);
        }

        bool TestOrdinal(string s)
        {
            if (number.Length >= 2 && number[1] == '1' && s.Equals ("TH", StringComparison.OrdinalIgnoreCase))
                return true;
            else
            {
                switch (number[number.Length -1])
                {
                    case '1':
                        return test["st"] (s);

                    case '2':
                        return test["nd"](s);

                    case '3':
                        return test["rd"] (s);

                    default:
                        return test["th"] (s);                    
                }
            }
        }

        public sealed class CardinalNumberExpression : TerminalSpan, PlainText
        {
            readonly string number;

            public CardinalNumberExpression(string number)
            {
                this.number = number;
            }

            public override string GenerateCode()
            {
                return number;
            }

            public string InnerText
            {
                get { return number; }
            }
        }

        sealed class OrdinalNumberExpression : TerminalSpan
        {
            readonly string number;
            readonly string ordinal;
            
            public OrdinalNumberExpression(string number, string ordinal_indicator)
            {
                this.number = number;
                this.ordinal = ordinal_indicator;
            }

            public override string GenerateCode()
            {
                return number + "<sup class=\"ordinal\">" + ordinal + "</sup>";
            }
        }

        sealed class DimesionExpression : TerminalSpan
        {
            readonly string number;

            public DimesionExpression(string number)
            {
                this.number = number;
            }
            
            public override string GenerateCode()
            {
                return number + " × ";
            }
        }

        sealed class DegreeExpression : TerminalSpan
        {
            readonly string number;

            public DegreeExpression(string number)
            {
                this.number = number;
            }

            public override string GenerateCode()
            {
                return number + "&#176";
            }
        }
    }
}