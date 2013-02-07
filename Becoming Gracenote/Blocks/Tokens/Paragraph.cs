namespace Gracenote
{
    public sealed class Paragraph : LineToken
    {
        public Paragraph(string line)
            : base (line)
        {

        }

        public override SyntaxNode StartNode(ref int i)
        {
            return new PartialParagraph (this);
        }
    }
}