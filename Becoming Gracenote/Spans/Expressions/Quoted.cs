using System.Collections.Generic;

namespace Gracenote
{
    public sealed class PartialSuperscriptExpression : NonTerminalSpan
    {
        readonly Indexical<SpanToken> index;

        public List<SpanToken> tokens = new List<SpanToken> ();

        public PartialSuperscriptExpression(Indexical<SpanToken> index)
        {
            this.index = index;
        }

        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var token = index.Value;

            if (token is Caret)
            {
                Pop ();
                return new SuperscriptExpression (this);
            }
            else
            {
                tokens.Add (token);
                
                return this;
            }
        }

        sealed class SuperscriptExpression : FromPartial<PartialSuperscriptExpression>
        {
            public SuperscriptExpression(PartialSuperscriptExpression partial) : base (partial) { }

            public override string GenerateCode()
            {
                return partial.tokens.Parse ().GenerateAndWrap ("sup");
            }
        }
    }

    public sealed class PartialSubscriptExpression : NonTerminalSpan
    {
        readonly Indexical<SpanToken> index;

        public PartialSubscriptExpression(Indexical<SpanToken> index)
        {
            this.index = index;
        }

        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            throw new System.NotImplementedException ();
        }
    }
}