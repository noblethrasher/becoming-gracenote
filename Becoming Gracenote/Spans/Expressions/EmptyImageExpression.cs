namespace Gracenote
{
    public sealed class EmptyImageExpression : TerminalSpan
    {

        public override string GenerateCode()
        {
            return "<form draggable=\"true\" class=\"gn_placeholder\" style=\"color:#666; display:inline-block; text-align:center; padding:10px; border:1px dashed #444;\">Drag and Drop or <button>Upload</button> an Image</form>";
        }
    }
}