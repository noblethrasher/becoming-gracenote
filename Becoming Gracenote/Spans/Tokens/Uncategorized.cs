namespace Gracenote
{
    using System;

    public sealed class UncategorizedToken : SpanToken
    {
        public UncategorizedToken(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new WordExpression (GetString ());
        }
    }
}