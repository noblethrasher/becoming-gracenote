using System.Collections.Generic;
namespace Gracenote
{
    public sealed class InlineDirectiveToken : SpanToken
    {
        bool finished = false;
        
        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            if (finished)
                return Start (ref index);
            
            var c = index.Value;

            if (c == ']')
            {
                finished = true;
                return this;
            }

            chars.Add (c);

            return this;
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return InlineDirectiveExpression.GetExpression (finished?  new string (chars.ToArray()) : "ERROR" );
        }
    }
}