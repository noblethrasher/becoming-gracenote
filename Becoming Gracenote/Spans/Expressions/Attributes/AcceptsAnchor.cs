namespace Gracenote
{
    public abstract class AcceptsAnchor : SyntaxAttribute
    {
        public abstract void Apply(string hyperReference, string innerText = null, string titleText = null);

        public bool IgnoreOpenInNewWindw { get; set; }
    }
}