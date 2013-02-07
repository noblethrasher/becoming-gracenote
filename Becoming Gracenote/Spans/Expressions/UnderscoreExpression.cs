namespace Gracenote
{
    public sealed class UnderscoreExpression : TerminalSpan
    {
        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            if (index.Value is Digits)
            {
                parentStack.Pop ();

                var num = index.Value.GetString();

                return new NumericSubscript (num);
            }
            
            else
                return base.Parse (ref index);
        }

        public override string GenerateCode()
        {
            return "_";
        }

        sealed class NumericSubscript : TerminalSpan
        {
            readonly string number;
            
            public NumericSubscript(string num)
            {
                number = num;
            }

            public override string GenerateCode()
            {
                return "<sub class=\"nsub\">" + number + "</sub>";
            }
        }
    }
}