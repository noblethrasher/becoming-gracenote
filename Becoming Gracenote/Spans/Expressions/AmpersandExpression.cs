namespace Gracenote
{
    public sealed class AmpersandExpression : TerminalSpan
    {
        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var upper = index + 2;

            if (upper)
            {
                var c0 = (index + 0).Value; //adding zero because I like the symmetry
                var c1 = (index + 1).Value;
                var c2 = (index + 2).Value;                

                if (c0 is Hash && c1 is Digits && c2 is Semicolon)
                {
                    parentStack.Pop ();

                    index = index + 2;

                    var line = c1.GetString ();

                    if (line.Length <= 7) //Since there are only 17 unicode planes with 2^16 characters each, let's just assume we can handle any number less that 9 million
                    {
                        var code = int.Parse(line);

                        return new HtmlEntityExpression (code);
                    }
                }
            }
            
            upper = index + 1;

            if (upper) //TODO: fix so that this can handle entities that are a mix of numbers and letters. ex &there4;
            {
                var c0 = (index + 0).Value;
                var c2 = (index + 1).Value;

                if (c0 is WordToken && c2 is Semicolon)
                {
                    index = index + 1;

                    parentStack.Pop ();

                    return new HtmlEntityExpression (c0.GetString ());
                }
            }               
            
            return base.Parse (ref index);
        }
        
        public override string GenerateCode()
        {
            return "&amp;";
        }

        public sealed class HtmlEntityExpression : TerminalSpan
        {
            readonly string entityCode;
            
            public HtmlEntityExpression(int number)
            {
                entityCode = "&#" + number + ";";
            }

            public HtmlEntityExpression(string name)
            {
                entityCode = "&" + name + ";";
            }

            public override string GenerateCode()
            {
                return entityCode;
            }
        }
    }
}