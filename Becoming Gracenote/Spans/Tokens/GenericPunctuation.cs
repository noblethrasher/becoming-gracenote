using System;
namespace Gracenote
{
    public sealed class GenericPunctuation : SpanToken
    {
        public GenericPunctuation(char c) : base (c)
        {
            if (!char.IsPunctuation (c))
                throw new ArgumentException ();
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericPunctuationExpression (chars[0]);
        }
    }
}