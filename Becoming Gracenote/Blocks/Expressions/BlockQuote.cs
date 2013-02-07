using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Gracenote
{
    public sealed class PartialBlockQuote : NonTerminalLine
    {
        readonly BlockQuote blockQuote;

        readonly List<string> lines = new List<string> ();

        readonly Regex leadingQuote;

        public PartialBlockQuote(BlockQuote blockQuote)
        {
            this.blockQuote = blockQuote;

            leadingQuote = new Regex (string.Format (@"^\s{0}>\s?", "{" + blockQuote.LeadingWhiteSpaceCount + "}"));

            lines.Add (StripQuoteCharacter (blockQuote));
        }

        public string StripQuoteCharacter(string line)
        {
            return leadingQuote.Replace (line, "", 1);
        }

        public override SyntaxNode Parse(LineToken token, ref int i)
        {
            if (token is BlockQuote)
            {
                lines.Add (StripQuoteCharacter (token));
                return this;
            }
            else
                return Complete (token, ref i);            
        }

        public SyntaxNode Complete(LineToken token, ref int i)
        {
            parentStack.Pop ();
            parentStack.Push (new CompeteQuoteExpression (this));

            return token.StartNode (ref i);
        }

        class CompeteQuoteExpression : TerminalLine
        {
            readonly PartialBlockQuote pq;

            public CompeteQuoteExpression(PartialBlockQuote pq)
            {
                this.pq = pq;
            }

            public override string GenerateCode(uint n = 0)
            {
                var sb = new StringBuilder ();

                sb.AppendLine ("<blockquote>", n);

                var parsed = new Parsed (new Tokenized (pq.lines));

                foreach (var exp in parsed)
                    sb.AppendLine (exp.GenerateCode (n + 1).TrimEnd());

                sb.AppendLine ("</blockquote>", n);

                return sb.ToString ();
            }
        }
    }
}