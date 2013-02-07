using System;

namespace Gracenote
{
    public abstract class AcceptsParagraph : BlockExpressionAttribute
    {
        public abstract SyntaxNode Accept(PartialParagraph.CompleteParagraph p);
    }
}