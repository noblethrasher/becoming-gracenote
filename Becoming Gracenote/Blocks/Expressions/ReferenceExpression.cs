using System.Collections.Generic;
using System.Text;
namespace Gracenote
{

    abstract class ReferenceExpression : TerminalLine
    {
        protected List<string> lines;
        protected string name;
        protected bool hidden;

        public static int order;

        public ReferenceExpression(string name, IEnumerable<string> lines, bool hidden)
        {
            this.name = name;
            this.lines = new List<string> (lines);
            this.hidden = hidden;
        }


    }


    sealed class UrlReferenceExpression : ReferenceExpression
    {
        new int order;
        
        public UrlReferenceExpression(string name, IEnumerable<string> lines, bool hidden) : base(name, lines, hidden)
        {
            if(!hidden)
                order = ++ReferenceExpression.order;
        }


        public override string GenerateCode(uint n = 0)
        {
            if (hidden)
                return "";
            
            return '\t'.Repeat(n) + "<p>" + '[' + (this.order) + ']' + " " + new Parsed (new[] { new TextFragmentToken (lines[0]) }).GenerateCode () + "</p>";
        }
    }

    
    
    sealed class ProseReferenceExpression : ReferenceExpression
    {
        new int order;
        
        public ProseReferenceExpression(string name, IEnumerable<string> lines, bool hidden) : base(name, lines, hidden)
        {
            if(!hidden)
                order = ++ReferenceExpression.order;
        }

        public override string GenerateCode(uint n = 0)
        {
            if (hidden)
                return "";
            
            var sb = new StringBuilder ();

            sb.AppendLine ("<dl class=\"referenceitem\">", n);

            sb.AppendLine ("<dt>[" + this.order + "]</dt>", n + 1);

            foreach (var line in lines)
                sb.AppendLine ("<dd>" + line + "</dd>", n + 1);

            sb.AppendLine ("</dl>");

            return sb.ToString ();
        }
    }
}