using System.Collections.Generic;
namespace Gracenote
{
    public sealed class PartialUnderlinedExpression : NonTerminalSpan
    {
        readonly Indexical<SpanToken> index;

        List<SpanToken> tokens = new List<SpanToken> ();

        public PartialUnderlinedExpression(Indexical<SpanToken> index)
        {
            this.index = index;
        }

        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var token = index.Value;

            if (token is DoubleUnderscore)
            {
                parentStack.Pop ();
                return new UnderlinedExpression (this);
            }
            else
            {
                tokens.Add (token);
                return this;
            }
        }

        sealed class UnderlinedExpression : FromPartial<PartialUnderlinedExpression>
        {
            public UnderlinedExpression(PartialUnderlinedExpression partial)
                : base (partial)
            {

            }

            public override string GenerateCode()
            {
                return "<u>" + new ParsedTextRun (partial.tokens).GenerateCode () + "</u>";
            }
        }
    }

    
}

