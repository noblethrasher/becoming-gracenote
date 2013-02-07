using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gracenote
{
    sealed class Email : SpanToken
    {
        readonly int length;
        int count;

        public Email(int length)
        {
            this.length = length;
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            if (++count > length)
                return Start (ref index);
            else
            {
                chars.Add (index.Value);
                return this;
            }                
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new EmailExpression (new string (chars.ToArray ()));
        }
    }
}
