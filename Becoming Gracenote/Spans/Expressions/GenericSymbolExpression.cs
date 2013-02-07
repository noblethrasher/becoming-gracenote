using System.Linq;

namespace Gracenote
{
    public sealed class GenericSymbolExpression : TerminalSpan
    {
        readonly char[] chars;
        
        public GenericSymbolExpression(char c)
        {
            chars = new[] { c };
        }

        public GenericSymbolExpression(params char[] cs)
        {
            chars = cs;
        }

        public override string GenerateCode()
        {
            return string.Join ("", chars.Select (c => "&#" + char.ConvertToUtf32 (c.ToString (), 0) + ";"));
        }
    }
}