namespace Gracenote
{
    public sealed class Directive : LineToken
    {
        public Directive(string line)
            : base (StripBrackets(line ?? ""))
        {

        }

        static string StripBrackets(string line)
        {
            var start = line.StartsWith("[") ? 1 : 0;
            var end = line.EndsWith ("]") ? 1 : 0;

            return line.Substring (start, line.Length - start - end);
        }

        public override SyntaxNode StartNode(ref int i)
        {
            return this.CreateDirective (i);
        }
    }
}