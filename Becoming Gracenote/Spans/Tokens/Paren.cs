using System;

namespace Gracenote
{
    public abstract class Paren : SpanToken
    {
        public Paren(char c)
            : base (c)
        {

        }
        
        public sealed override SpanToken Scan(ref Indexical<CharEx> index)
        {
 	        return Start(ref index);
        }
    }
    
    public sealed class LeftParen : Paren
    {
        public LeftParen(char c)
            : base (c)
        {
            if (c != '(')
                throw new ArgumentException ();
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new PartialParenthetical (index);
        }
    }

    public sealed class RightParen : Paren
    {
        public RightParen(char c)
            : base (c)
        {
            if (c != ')')
                throw new ArgumentException ();
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericSymbolExpression (chars[0]);
        }
    }
}