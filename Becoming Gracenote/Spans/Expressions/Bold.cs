using System.Collections.Generic;
using System.Linq;

namespace Gracenote
{
    public sealed class PartialBoldExpression : Container<DoubleAsterisk, DoubleAsterisk>
    {
        public PartialBoldExpression(Indexical<SpanToken> index)
            : base (index)
        {

        }

        protected override IEnumerable<SpanToken> GetStartSymbol()
        {
            return new[] { new LiteralToken ('*'), new LiteralToken ('*') };
        }

        protected override TerminalSpan GetCompletion()
        {
            return new Bold (this);
        }

        class Bold : TerminalSpan
        {
            readonly PartialBoldExpression partial;

            public Bold(PartialBoldExpression partial)
            {
                this.partial = partial;
            }

            public override string GenerateCode()
            {
                return "<strong>" + GenerateSubCode (partial) + "</strong>";
            }
        }
    }    
}
