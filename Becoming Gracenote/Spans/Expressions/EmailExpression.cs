using System;
using System.Linq;

namespace Gracenote
{
    public sealed class EmailExpression : TerminalSpan
    {
        readonly string email;

        public string Email
        {
            get
            {
                return email;   
            }
        }

        public EmailExpression(string email)
        {
            this.email = email;

            if (Parsed.WordCountAccumulator != null)
                Parsed.WordCountAccumulator (email);
        }

        protected override void OnRegistration(StackEx<SpanSyntaxNode> stack)
        {
            var neighbor = GetNeighbor ();

            if (neighbor != null)
            {
                var non_wp = neighbor.GetImmediatelyNearestNonWhitesSpace ();

                if (non_wp != null)
                {
                    var attr = non_wp.GetAttribute<AcceptsAnchor> ();

                    if (attr != null)
                    {
                        parentStack.Pop ();

                        if (neighbor is PartialWhitespaceExpression.WhitespaceExpression)
                            parentStack.Pop ();

                        attr.IgnoreOpenInNewWindw = true;

                        attr.Apply ("mailto:" + EscapeChars(email));
                    }
                }
            }
        }

        string EscapeChars(string s)
        {
            var xs = from c in s
                     select char.IsLetterOrDigit (c) ? c.ToString () : "&#" + char.ConvertToUtf32 (c.ToString (), 0) + ";";

            return string.Join ("", xs);
        }

        public override string GenerateCode()
        {
            return "<a href=\"mailto:" + EscapeChars(email) + "\">" + email + "</a>";
        }
    }
}