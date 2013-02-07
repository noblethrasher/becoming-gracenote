using System.Collections.Generic;
using System.Linq;
namespace Gracenote
{
    public sealed class PartialBoldItalicExpression : Container<TripleAsterisk, TripleAsterisk>
    {
        public PartialBoldItalicExpression(Indexical<SpanToken> index)
            : base (index)
        {

        }
        
        protected override IEnumerable<SpanToken> GetStartSymbol()
        {
            return new[] { new LiteralToken ('*'), new LiteralToken ('*'), new LiteralToken ('*') };
        }

        protected override TerminalSpan GetCompletion()
        {
            return new BoldItalic (this);
        }

        class BoldItalic : TerminalSpan
        {
            readonly PartialBoldItalicExpression partial;

            public BoldItalic(PartialBoldItalicExpression partial)
            {
                this.partial = partial;
            }

            public override string GenerateCode()
            {
                return "<strong><em>" + GenerateSubCode (partial) + "</em></strong>";
            }
        }
    }
}