namespace Gracenote
{
    public sealed class GenericPunctuationExpression : TerminalSpan
    {
        readonly char @char;
        
        public GenericPunctuationExpression(char c)
        {
            this.@char = c;
        }

        public override string GenerateCode()
        {
            return "&#" + char.ConvertToUtf32 (@char.ToString (), 0) + ";";
        }
    }
}