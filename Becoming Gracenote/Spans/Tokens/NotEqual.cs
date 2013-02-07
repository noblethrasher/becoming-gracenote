using System;

namespace Gracenote
{
    class NotEqual : SpanToken
    {
        public NotEqual(char exclamation, char equals)
            : base (exclamation)
        {
            Add (equals);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new NotEqualExpression ();
        }
    }

    class AlmostEqual : SpanToken
    {
        public AlmostEqual(char tilde, char equals)
            : base (tilde)
        {
            Add (equals);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new AlmostEqualExpression ();
        }
    }

    class GreaterThanOrEqual : SpanToken
    {
        public GreaterThanOrEqual(char rightAngleBracket, char equals) : base(rightAngleBracket)
        {
            if (rightAngleBracket != '>' || equals != '=')
                throw new ArgumentException ();

            Add (equals);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GreaterThanOrEqualExpression ();
        }
    }

    class LessThanOrEqual : SpanToken
    {
        public LessThanOrEqual(char leftAngleBracket, char equals)
            : base (leftAngleBracket)
        {
            if (leftAngleBracket != '<' || equals != '=')
                throw new ArgumentException ();

            Add (equals);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new LessThanOrEqualExpression ();
        }
    }
}
