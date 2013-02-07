using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Gracenote
{
    public sealed class PartialParagraph : NonTerminalLine
    {
        readonly Stack<Paragraph> ps = new Stack<Paragraph> ();
    
        public PartialParagraph(Paragraph paragraph)
        {
            //this.paragraph = paragraph;

            ps.Push (paragraph);
        }

        public override SyntaxNode Parse(LineToken token, ref int i)
        {
            if (token is Paragraph)
            {
                ps.Push (token as Paragraph);
                return this;
            }
            else
            {
                parentStack.Pop ();

                var header_factory = (token as HorizontalRule).GetHeaderFactory (ps.Peek());

                if (header_factory != null)
                {

                    if (ps.Count > 1)
                    {
                        var top = ps.Pop ();

                        parentStack.Push (new CompleteParagraph (this));

                        return header_factory (top);
                    }
                    else
                    {
                        return header_factory (ps.Peek ());
                    }
                    
                }
                else
                {
                    parentStack.Push (new CompleteParagraph (this));
                    return parentStack.Top.Parse (token, ref i);
                }
            }
        }

        public sealed class CompleteParagraph : TerminalLine
        {
            readonly PartialParagraph paragraph;
            bool seenLine = false;
            
            public CompleteParagraph(PartialParagraph p)
            {
                this.paragraph = p;

                attributes.Add (new MakeParapgraphList (this));
            }

            protected override void OnRegistration(StackEx<SyntaxNode> stack)
            {
                var neighbor = stack.TopNode.Next;

                if (neighbor != null)
                {
                    var acceptsParagraph = neighbor.Value.GetAttribute<AcceptsParagraph> ();

                    if (acceptsParagraph != null)
                    {
                        parentStack.Pop ();

                        parentStack.Push (acceptsParagraph.Accept (this));
                    }
                }
            }

            public override SyntaxNode Parse(LineToken token, ref int i)
            {
                seenLine = false;
                
                if (token.NumberOfBlankLines == 1 && !seenLine)
                {   
                    seenLine = true;
                    return this;
                }
                
                return base.Parse (token, ref i);
            }

            sealed class MakeParapgraphList : AcceptsParagraph
            {
                readonly CompleteParagraph p;

                public MakeParapgraphList(CompleteParagraph p)
                {
                    this.p = p;
                }
                
                public override SyntaxNode Accept(CompleteParagraph p)
                {
                    p.parentStack.Pop ();
                    
                    var pList = new ParagraphList (this.p);
                    pList.Add (p);

                    p.parentStack.Push (pList);

                    return pList;
                }
            }

            public override string GenerateCode(uint n = 0)
            {
                return GenerateCode (n, null);
            }

            public string GenerateCode(uint n = 0, IEnumerable<string> classes = null)
            {
                var cssClasses = classes != null ? string.Join (" ", classes) : "";

                if (cssClasses.Length > 0)
                    cssClasses = " class=\"" + cssClasses + "\"";

                var sb = new StringBuilder ();


                var paragraphs = this.paragraph.ps.Reverse ().ToList();

                if (paragraphs.Count < 2)
                {
                    sb.AppendLine (string.Format ("<p{0}>", cssClasses) + string.Join ("", paragraphs.Select (p => new ParsedTextRun (p).GenerateCode ())) + "<div class=\"pcl\" /></p>", n);
                }
                else
                {
                    var lnbrk = paragraph.ps.Count > 1 ? "<br />" : "";
                    sb.AppendLine (string.Format ("<p{0}>", cssClasses), n);

                    for (var i = 0; i < paragraphs.Count - 1; i++)
                        sb.AppendLine (new ParsedTextRun (paragraphs[i]).GenerateCode () + lnbrk, n + 1);

                    sb.AppendLine (new ParsedTextRun (paragraphs[paragraphs.Count - 1]).GenerateCode (), n + 1);

                    sb.AppendLine ("<div class=\"pcl\" /></p>", n);
                }

                return sb.ToString ();
            }
        }

        public sealed class ParagraphList : TerminalLine
        {
            readonly List<CompleteParagraph> paragraphs = new List<CompleteParagraph> ();

            bool seenLine;

            static readonly Dictionary<int, string> cssClass = new Dictionary<int, string> ()
            {
                { 1, "odd" },
                { 0, "even" }
            };
            
            public ParagraphList(CompleteParagraph p)
            {
                paragraphs.Add (p);

                attributes.Add (new AcceptParagraph (this));
            }

            public void Add(CompleteParagraph p)
            {
                paragraphs.Add (p);
            }

            public override SyntaxNode Parse(LineToken token, ref int i)
            {
                if (token.NumberOfBlankLines == 1 && !seenLine)
                {
                    seenLine = true;
                    return this;
                }                
                
                return base.Parse (token, ref i);
            }

            public override string GenerateCode(uint n = 0)
            {
                var sb = new StringBuilder ();

                int i = 1;

                sb.AppendLine ("<div class=\"paragraph-run\">", n);

                foreach (var p in paragraphs)
                {
                    sb.AppendLine (p.GenerateCode (classes: new[] { cssClass[i % 2], "n" + i }), n + 1);
                    i++;
                }

                sb.AppendLine ("</div>");

                return sb.ToString ();
            }

            sealed class AcceptParagraph : AcceptsParagraph
            {
                readonly ParagraphList pList;

                public AcceptParagraph(ParagraphList pList)
                {
                    this.pList = pList;
                }
                
                public override SyntaxNode Accept(CompleteParagraph p)
                {
                    pList.seenLine = false;
                    
                    pList.Add (p);

                    return pList;
                }
            }
        }
    }
}