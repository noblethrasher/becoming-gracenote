using System;
namespace Gracenote
{
    public abstract class HorizontalRule : LineToken
    {
        private HorizontalRule(string line)
            : base (line)
        {

        }

        public sealed override SyntaxNode StartNode(ref int i)
        {
            return new HorizontalRuleExpression (this);
        }

        public abstract HeaderExpression HeaderFactory(string line);

        sealed class FromEqualSign : HorizontalRule
        {
            public FromEqualSign(string line) : base (line) { }
            
            public override HeaderExpression HeaderFactory(string line)
            {
                return new HeaderExpression.H1 (line);
            }
        }

        sealed class FromDash : HorizontalRule
        {
            public FromDash(string line) : base (line) { }
            
            public override HeaderExpression HeaderFactory(string line)
            {
                return new HeaderExpression.H2 (line);
            }
        }

        sealed class FromUnderScore : HorizontalRule
        {
            public FromUnderScore(string line) : base (line) { }
            
            public override HeaderExpression HeaderFactory(string line)
            {
                return new HeaderExpression.H3 (line);
            }
        }

        sealed class FromPlus : HorizontalRule
        {
            public FromPlus(string line) : base (line) { }

            public override HeaderExpression HeaderFactory(string line)
            {
                return new HeaderExpression.H4 (line);
            }
        }

        sealed class FromTilde : HorizontalRule
        {
            public FromTilde(string line) : base (line) { }

            public override HeaderExpression HeaderFactory(string line)
            {
                return new HeaderExpression.H5 (line);
            }
        }

        public static LineToken Create(string line)
        {
            if (line == null || line.Length == 0)
                throw new ArgumentException ();

            var c = line[0];

            switch (c)
            {
                case '=':
                    return new FromEqualSign (line);
                
                case '-':
                    return new FromDash(line);
                
                case '_':
                    return new FromUnderScore (line);

                case '+':
                    return new FromPlus (line);

                case '~':
                    return new FromTilde (line);
                    
                
                default:
                    throw new ArgumentException ();
            }
        }
    }

    public static class HeaderModule
    {
        public static Func<string, HeaderExpression> GetHeaderFactory(this HorizontalRule header, string line)
        {
            if (header == null)
                return null;
            
            string header_txt = header;

            var epsilon = line.Length * 0.1;

            if (!(Math.Abs (line.Length - header_txt.Length) <= epsilon))
                return null;
            else
                return header.HeaderFactory;             
        }
    }
}