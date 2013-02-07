using System.Collections.Generic;

namespace Gracenote
{
    abstract class ReferenceItemToken : LineToken
    {
        internal readonly List<string> lines = new List<string> ();
        internal readonly string name;
        internal bool hidden = false;

        public ReferenceItemToken(string name, string line) : base(line)
        {

            System.Diagnostics.Trace.WriteLine ("Ref Token Created: " + name);

            var s = line.Substring (('[' + name + ']').Length).Trim ();

            System.Diagnostics.Trace.WriteLine (s + "," + name);

            lines.Add (s);
            
            if (name.StartsWith ("!") && name.Length > 1)
            {
                this.name = name.Substring (1);
                hidden = true;
            }
            else
                this.name = name;
        }

        public abstract string ReplaceReferences(string line);

        public static ReferenceItemToken Create(string name, string line)
        {
            if (line.Substring (name.Length - 1).LooksLikeURL ())
                return new URLReferenceToken (name, line);
            else
                return new GenericReferenceToken (name, line);
        }

        //public override LineToken Lex(string line, ref int i)
        //{
        //    if (Utils.URI_Pattern.IsMatch (line))
        //    {
        //        lines.Add (line);
        //        return this;
        //    }
        //    else
        //        return Start (line, ref i);
        //}

        sealed class URLReferenceToken : ReferenceItemToken
        {
            public URLReferenceToken(string name, string line) 
                : base(name, line)
            {
                
            }

            public override SyntaxNode StartNode(ref int i)
            {
                return new UrlReferenceExpression (name, lines, hidden);
            }

            public override string ReplaceReferences(string line)
            {
                return line.Replace ('[' + name + ']', "(" + lines[0] + ")");
            }
        }

        sealed class GenericReferenceToken : ReferenceItemToken
        {
            public GenericReferenceToken(string name, string line)
                : base (name, line)
            {
                
            }

            public override SyntaxNode StartNode(ref int i)
            {
                return new ProseReferenceExpression (name, lines, hidden);
            }

            public override string ReplaceReferences(string line)
            {
                return line;
            }
        }
    }
}