namespace Gracenote
{
    public abstract class HandlesAcronym : SyntaxAttribute
    {
        public abstract TerminalSpan Handle(string text);
    }
}