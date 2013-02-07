using System.Collections.Generic;
using System.Linq;

namespace Gracenote
{
    public sealed class PartialBackTickQuotedExpression : Container<GraveAccent, GraveAccent>
    {
        public PartialBackTickQuotedExpression(Indexical<SpanToken> index)
            : base (index)
        {

        }

        protected override IEnumerable<SpanToken> GetStartSymbol()
        {
            return AsLiterals ('`');
        }

        protected override TerminalSpan GetCompletion()
        {
            return new BackTickQuotedExpression (this);
        }

        class BackTickQuotedExpression : TerminalSpan
        {
            readonly PartialBackTickQuotedExpression partial;

            public BackTickQuotedExpression(PartialBackTickQuotedExpression partial)
            {
                this.partial = partial;

                attributes.Add (new _AccpetHyperlinkCapability (this));
            }

            sealed class _AccpetHyperlinkCapability : AcceptsAnchor
            {
                readonly BackTickQuotedExpression exp;

                public _AccpetHyperlinkCapability(BackTickQuotedExpression exp)
                {
                    this.exp = exp;
                }
                
                public override void Apply(string hyperReference, string innerText = null, string titleText = null)
                {
                    var text = exp.GenerateCode ();

                    var escaped_text = from c in titleText ?? ""
                                       let e = char.IsLetterOrDigit (c) || char.IsWhiteSpace(c) ? c.ToString() : "&#" + char.ConvertToUtf32 (c.ToString (), 0).ToString () + ";"
                                       select e;


                    titleText = string.Join ("", escaped_text);

                    var href = new HyperLinkExpression (hyperReference, text, titleText);

                    href.IgnoreOpenInNewWindow = this.IgnoreOpenInNewWindw;

                    exp.node.Value = href;
                }
            }

            public override string GenerateCode()
            {
                return GenerateSubCode (partial);
            }
        }
    }

    public sealed class PartialDoubleBackTickQuotedExpression : Container<DoubleGraveAccent, DoubleGraveAccent>
    {
        public PartialDoubleBackTickQuotedExpression(Indexical<SpanToken> index)
            : base (index)
        {

        }

        protected override IEnumerable<SpanToken> GetStartSymbol()
        {
            return AsLiterals ('`', '`');
        }

        protected override TerminalSpan GetCompletion()
        {
            return new DoubleBackTickQuotedExpression (this);
        }

        class DoubleBackTickQuotedExpression : TerminalSpan
        {
            readonly PartialDoubleBackTickQuotedExpression partial;

            public DoubleBackTickQuotedExpression(PartialDoubleBackTickQuotedExpression partial)
            {
                this.partial = partial;
            }

            public override string GenerateCode()
            {
                return "<code>" + GenerateSubCode (partial) + "</code>";
            }
        }
    }    
}