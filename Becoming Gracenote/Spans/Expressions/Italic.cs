using System.Collections.Generic;
using System.Linq;

namespace Gracenote
{
    public sealed class PartialItalic : Container<Asterisk, Asterisk>
    {

        public PartialItalic(Indexical<SpanToken> index) : base(index)
        {

        }
        
        protected override IEnumerable<SpanToken> GetStartSymbol()
        {
            return new[] { new LiteralToken ('*') };
        }

        protected override TerminalSpan GetCompletion()
        {
            return new Italic (this);
        }

        class Italic : TerminalSpan
        {
            readonly PartialItalic partial;

            public Italic(PartialItalic partial)
            {
                this.partial = partial;                
            }

            public override string GenerateCode()
            {
                return "<em>" + GenerateSubCode (partial) + "</em>";
            }
        }
    }
}
