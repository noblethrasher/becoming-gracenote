using System.Collections.Generic;
using System.Linq;

namespace Gracenote
{
    public sealed class PartialWhitespaceExpression : NonTerminalSpan
    {
        readonly List<char> chars = new List<char> ();

        public PartialWhitespaceExpression(IEnumerable<char> xs)
        {
            this.chars.AddRange (xs);
        }

        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            Pop ();
            parentStack.Push (new WhitespaceExpression (this));

            return parentStack.Top.Parse (ref index);
        }

        internal sealed class WhitespaceExpression : FromPartial<PartialWhitespaceExpression>, PlainText
        {
            public WhitespaceExpression(PartialWhitespaceExpression partial) : base(partial)
            {
                
            }

            public override string GenerateCode()
            {
                return new string (this.partial.chars.ToArray ());
            }

            public string InnerText
            {
                get { return GenerateCode (); }
            }
        }
    }
}