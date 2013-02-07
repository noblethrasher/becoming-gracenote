using System;

namespace Gracenote
{
    public abstract class Braket : SpanToken
    {
        public Braket(char c)
            : base (c)
        {

        }

        public sealed override SpanToken Scan(ref Indexical<CharEx> index)
        {
            return Start (ref index);
        }
    }

    [Guard('[')]
    public sealed class LeftBraket : Braket
    {
        public LeftBraket(char c)
            : base (c)
        {
            
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericSymbolExpression (chars[0]);
        }
    }

    [Guard (']')]
    public sealed class RightBraket : Braket
    {
        public RightBraket(char c)
            : base (c)
        {
            
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericSymbolExpression (chars[0]);
        }
    }
}