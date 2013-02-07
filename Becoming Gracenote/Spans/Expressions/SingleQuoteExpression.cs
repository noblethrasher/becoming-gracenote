using System.Collections.Generic;
namespace Gracenote
{
    public sealed class ApostropheExpression : TerminalSpan
    {
        readonly Indexical<SpanToken> index;
        
        public ApostropheExpression(Indexical<SpanToken> index)
        {
            this.index = index;
        }
        
        public override string GenerateCode()
        {
            return "&#146;";
        }
    }

    public sealed class SingleQuoteExpression : NonTerminalSpan
    {
        List<SpanToken> tokens = new List<SpanToken> ();

        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var token = index.Value;

            if (token is DoublePrime)
            {
                parentStack.Pop ();
                
                return new CompleteSingleQuoteExpression (tokens);
            }
            else
            {
                tokens.Add (token);
                return this;
            }
        }

        sealed class CompleteSingleQuoteExpression : TerminalSpan
        {
            readonly IEnumerable<SpanToken> tokens;

            public CompleteSingleQuoteExpression(IEnumerable<SpanToken> tokens)
            {
                this.tokens = tokens;
            }

            public override string GenerateCode()
            {
                var parsed = new ParsedTextRun (tokens);

                return "&#145;" + parsed.GenerateCode () + "&#146;";
            }
        }
    }
}