namespace Gracenote
{
    [Guard('\\')]
    public class PartialLiteralToken : SpanToken
    {
        public PartialLiteralToken(char c)
            : base (c)
        {

        }

        class DimensionSign : SpanToken
        {
            sealed class DimesnionSignExpression : TerminalSpan
            {
                public override string GenerateCode()
                {
                    return "&times;";
                }
            }
            
            public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
            {
                return new DimesnionSignExpression ();
            }
        }

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            if (index + 3)
            {
                if (index.Value == 'm')
                    if ((index + 1).Value == 'u')
                        if ((index + 2).Value == 'l')
                            if ((index + 3).Value == 't')
                            {
                                index = index + 3;
                                parentStack.Pop ();

                                return new DimensionSign ();
                            }
            }
            
            parentStack.Pop ();
            return new LiteralToken (index.Value);
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            return new GenericSymbolExpression (chars[0]);
        }
    }

    public class LiteralToken : SpanToken
    {
        public LiteralToken(char c)
            : base (c)
        {

        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            var c = chars[0];

            if (char.IsSymbol (c))
                return new GenericSymbolExpression (c);

            if (char.IsPunctuation (c))
                return new GenericPunctuationExpression (c);

            return new WordExpression (c.ToString ());
        }
    }
}