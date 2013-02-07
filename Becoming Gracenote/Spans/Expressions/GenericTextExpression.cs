using System.Collections.Generic;
using System.Text;
namespace Gracenote
{
    public sealed class TextFragment : TerminalSpan
    {
        readonly IEnumerable<TextFragment.Item> items;
        
        public TextFragment(IEnumerable<TextFragment.Item> items)
        {
            this.items = items;
        }

        public override string GenerateCode()
        {
            var sb = new StringBuilder ();

            foreach (var item in items)
                sb.Append(item.GenerateCode());

            return sb.ToString();

        }

        public abstract class Item
        {
            public abstract string GenerateCode();
            
            public class SubTerminal : Item
            {
                readonly TerminalSpan node;

                public SubTerminal(TerminalSpan node)
                {
                    this.node = node;
                }

                public override string GenerateCode()
                {
                    return node.GenerateCode ();
                }
            }

            public class SubText : Item
            {
                readonly List<SpanToken> tokens = new List<SpanToken>();

                public SubText(SpanToken token)
                {
                    tokens.Add (token);
                }

                public SubText(IEnumerable<SpanToken> tokens)
                {
                    this.tokens.AddRange (tokens);
                }

                public SubText()
                {

                }

                public override string GenerateCode()
                {
                    return new ParsedTextRun (tokens).GenerateCode ();
                }
            }
        }
    }
}