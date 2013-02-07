using System.Collections.Generic;
using System.Linq;

namespace Gracenote
{
    public static class SpanParseHelper
    {
        public static ParsedTextRun Parse(this IEnumerable<SpanToken> tokens)
        {
            return new ParsedTextRun (tokens);
        }
    }
    
    public class ParsedTextRun : IEnumerable<TerminalSpan>
    {
        IEnumerable<TerminalSpan> nodes;

        public ParsedTextRun(string text) : this(new TokenizedSpans(text))
        {

        }

        public ParsedTextRun(IEnumerable<SpanToken> tokens)
        {
            if (tokens.Any ())
            {
                var list = tokens.Union(new[] { new EndOfline() }).ToArray();

                var index = new Indexical<SpanToken> (list);

                var stack = new StackEx<SpanSyntaxNode> (list[0].GetStartNode (ref index));

                while (++index)
                {
                    stack.Push (stack.Top.Parse (ref index));
                }

                nodes = from n in stack.Reverse ()
                        let t = n as TerminalSpan
                        where t != null
                        select t;
            }
            else
            {
                nodes = Enumerable.Empty<TerminalSpan> ();
            }
        }

        public string GenerateCode()
        {
            return string.Join("", (from node in nodes select node.GenerateCode()));
        }

        public string GenerateAndWrap(string tag)
        {
            return "<" + tag + ">" + GenerateCode () + "</" + tag + ">";
        }

        public IEnumerator<TerminalSpan> GetEnumerator()
        {
            return nodes.GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }
    }
}