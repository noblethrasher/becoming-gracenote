using System;


namespace Gracenote
{    
    sealed class Trademark : SpanToken
    {
        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new TrademarkExpression ();
        }
    }

    sealed class RegisteredTrademark : SpanToken
    {
        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new RegisteredTrademarkExpression ();
        }
    }

    sealed class Copyright : SpanToken
    {
        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new CopyrightExpression ();
        }
    }
}
