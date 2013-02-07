using System;
namespace Gracenote
{
    public sealed class EndOfline : SpanToken
    {
        public EndOfline() : base((char) 26)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            throw new Exception ("Unexpected end of line");
        }
    }
}