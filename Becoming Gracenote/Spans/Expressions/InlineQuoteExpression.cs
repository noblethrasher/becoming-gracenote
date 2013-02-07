using System.Linq;
using System.Collections.Generic;

namespace Gracenote
{
    public sealed class PartialInlineQuoteExpression : Container<DoubleQuote, DoubleQuote>
    {
        public PartialInlineQuoteExpression(Indexical<SpanToken> index)
            : base (index)
        {

        }

        protected override IEnumerable<SpanToken> GetStartSymbol()
        {
            return new[] { new LiteralToken ('"') };
        }

        protected override TerminalSpan GetCompletion()
        {
            return new InlineQuote (this);
        }

        public sealed class InlineQuote : TerminalSpan
        {
            readonly PartialInlineQuoteExpression partial;

            public InlineQuote(PartialInlineQuoteExpression partial)
            {
                this.partial = partial;
            }

            public string GetString()
            {
                return GenerateSubCode (partial);
            }

            public override string GenerateCode()
            {
                return "&#8220;" + GetString() + "&#8221;";
            }
        }
    }    
}