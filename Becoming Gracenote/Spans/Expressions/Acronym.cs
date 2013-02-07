namespace Gracenote
{
    public sealed class Acronym : TerminalSpan
    {
        readonly string word, text;

        public Acronym(string word, string text)
        {
            this.word = word;
            this.text = text;
        }
        
        public override string GenerateCode()
        {
            return "<abbr title=\"" + text + "\">" + word + "</abbr>";
        }
    }
}