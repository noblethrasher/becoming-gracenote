namespace Gracenote
{
    using System.Collections.Generic;
    
    public class ListToken : LineToken
    {
        readonly int leading;
        public readonly ListStyleType list_style;

        public ListToken(string line, int leading, ListStyleType list_style)
            : base (line)
        {
            this.leading = leading;
            this.list_style = list_style;
        }

        public override SyntaxNode StartNode(ref int i)
        {
            return new PartialList (this);
        }

        public string GetProperString()
        {
            return Utils.ListPattern.Replace (line, "", 1);
        }

        public int SpacesToTrim 
        {
            get
            {
                return leading;
            }
        }
    }
}