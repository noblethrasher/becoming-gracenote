namespace Gracenote
{
    using System.Collections.Generic;
    
    public sealed class Blank : LineToken
    {
        List<string> lines = new List<string> ();
        
        public Blank(string line)
            : base (line)
        {
            lines.Add (line);
        }        

        public override LineToken Lex(string line, ref int i)
        {
            if (IsBlank (line))
            {
                lines.Add (line);
                return this;
            }

            return Start (line, ref i);
        }

        public override int NumberOfBlankLines
        {
            get
            {
                return lines.Count;
            }
        }

        public override SyntaxNode StartNode(ref int i)
        {
            return new BlankLineExpression (this);
        }
    }
}