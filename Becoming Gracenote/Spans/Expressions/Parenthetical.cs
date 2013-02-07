using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gracenote
{
    public sealed class PartialParenthetical : Container<LeftParen, RightParen>
    {
        public PartialParenthetical(Indexical<SpanToken> index, PartialParenthetical partialParenthetical = null) : base(index, partialParenthetical) 
        {
            
        }

        public sealed class Parenthetical : TerminalSpan
        {
            readonly PartialParenthetical partial;
            
            public Parenthetical(PartialParenthetical partial)
            {
                this.partial = partial;                
            }

            protected override void OnRegistration(StackEx<SpanSyntaxNode> stack)
            {
                var xs = from itm in partial.items
                         from term in itm.GetTerminals()
                         select term;
                
                var child_terminals = xs.ToArray();

                if(child_terminals.Length != 1 && child_terminals.Length != 2)
                    return;

                var href = child_terminals[0] as HyperLinkExpression;

                if (href == null)
                    goto EMAIL;

                var title = child_terminals.Length == 2 ? child_terminals[1] as PartialInlineQuoteExpression.InlineQuote : null;

                var some_terminal = GetNeighbor ();

                if (some_terminal != null)
                {
                    var non_whitespace = some_terminal.GetImmediatelyNearestNonWhitesSpace ();

                    if (non_whitespace != null)
                    {

                        var handles_acronym = some_terminal.GetAttribute<HandlesAcronym> ();

                        if (handles_acronym != null)
                        {
                            parentStack.Pop ();

                            var textuals = from exp in child_terminals
                                           let pt = exp as PlainText
                                           where pt != null
                                           select pt.InnerText;

                            var text = string.Join ("", textuals);

                            parentStack.Push (handles_acronym.Handle (text));
                            return;
                        }

                        if (title == null)
                            return;
                        
                        var handle_href = some_terminal.GetAttribute<AcceptsAnchor> ();

                        if (handle_href != null)
                        {
                            parentStack.Pop ();

                            handle_href.Apply (href.HREF, titleText: title.GetString ());
                        }
                    }
                }

                EMAIL:

                var email = child_terminals[0] as EmailExpression;

                if (email != null && child_terminals.Length == 1)
                {
                    var some_term = GetNeighbor ();

                    if (some_term != null)
                    {
                        var non_whitespace = some_term.GetImmediatelyNearestNonWhitesSpace ();

                        if (non_whitespace != null)
                        {
                            var attr = non_whitespace.GetAttribute<AcceptsAnchor> ();

                            if (attr != null)
                            {
                                parentStack.Pop ();

                                if (some_term is PartialWhitespaceExpression.WhitespaceExpression)
                                    parentStack.Pop ();

                                attr.IgnoreOpenInNewWindw = true;
                                
                                attr.Apply ("mailto:" + email.Email.Replace ("\"", "&quot;"));
                            }
                        }
                    }
                }
            }

            public override string GenerateCode()
            {
                return "(" + GenerateSubCode (partial) + ")";
            }
        }

        protected override IEnumerable<SpanToken> GetStartSymbol()
        {
            return new[] { new LiteralToken ('(') };
        }        

        protected override TerminalSpan GetCompletion()
        {
            return new Parenthetical (this);
        }        
    }
}