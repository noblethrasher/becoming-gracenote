using System;
namespace Gracenote
{
    public sealed class GenericSymbol : SpanToken
    {
        public GenericSymbol(char c)
            : base (c)
        {
            if (!char.IsSymbol (c))
                throw new ArgumentException ();
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericSymbolExpression (chars[0]);
        }
    }
}