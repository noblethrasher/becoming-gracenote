using System.Collections.Generic;
using System.Linq;
using System;

namespace Gracenote
{
    public static class InlineDirectiveExpression
    {
       public static TerminalSpan GetExpression(string s)
        {
            if (s != null && s.Length > 0)
            {
                if (s.ToUpper () == "STRIKE")
                    return new StrikeExpression ();                
                
                var split = s.Split (':');

                string directive, data;

                var index = s.IndexOf (':');

                directive = s.Substring (0, index).ToUpper();
                data = s.Substring (index + 1);




                if (split.Length > 1)
                {
                    switch(directive)
                    {
                        case "IMG":
                        case "IMAGE":
                        case "PIC":
                        case "PICTURE":
                        case "FIG":
                        case "FIGURE":
                            {
                                return new InlineImageExpression (data);
                            }

                        case "CLIP":
                            return MediaClipExpression.Create (data);

                        case "CORRECTION":
                            return new CorrectionExpression (data);

                        default:
                            return new UnknownInlineDirective ();
                    }
                }
            }            
            
            return new UnknownInlineDirective ();
        }

       abstract class MediaClipExpression : TerminalSpan
       {
           public static MediaClipExpression Create(string data)
           {
               return null;
           }
       }

        sealed class InlineImageExpression : TerminalSpan
        {
            readonly string title, path, element_id;

            bool is_first = false;
            bool is_last = false;

            struct ImgData
            {
                public string Title;
                public string ID;
                public string Path;

                public ImgData (string title, string element_id, string path)
                {
                    this.Title = title;
                    this.ID = element_id;
                    this.Path = path;
                }
            }

            ImgData ParseImageData(string s)
            {
                var chars = s.ToCharArray ();

                var data = new ImgData ();

                for (var i = 0; i < chars.Length; i++)
                {
                    var c = chars[i];
                    
                    if (c == TokenizedSpans.naked_url_sigil[0])
                    {
                        var sub = chars.Skip (i).Take (TokenizedSpans.naked_url_sigil.Length).ToArray();

                        var equal = sub.SequenceEqual (TokenizedSpans.naked_url_sigil);

                        if (equal)
                        {
                            i = i + sub.Length;

                            var path = new List<char> ();

                            while (i < chars.Length && !char.IsWhiteSpace (chars[i]))
                            {
                                var k = chars[i];
                                i++;

                                if (k == '\\')
                                    break;
                                else

                                path.Add (k);
                            }

                            data.Path = new string (path.ToArray ());
                        }
                    }

                    if (c == '"')
                    {
                        var sub = chars.Skip(i).ToArray();

                        if (sub.Length > 1 && sub.Contains('"'))
                        {
                            var title = new List<char> ();

                            while (++i < chars.Length && chars[i] != '"')
                            {
                                title.Add (chars[i]);
                            }

                            data.Title = new string (title.ToArray ());
                        }
                    }
                    else
                    if (!char.IsWhiteSpace (c) && data.Path == null)
                    {
                        var _path = new List<char> ();

                        while (i < chars.Length && !char.IsWhiteSpace (chars[i]) && chars[i] != ']' && chars[i] != '"')
                        {
                            _path.Add (chars[i]);
                            i++;
                        }

                        data.Path = Parsed.RelativeMediaPath + new string (_path.ToArray ());
                    }
                }

                return data;
            }

            public InlineImageExpression(string data)
            {
                //var result = ImageDataParser.Parse (data);

                var result = ParseImageData (data);

                element_id = result.ID;
                title = result.Title;
                path = result.Path;
            }

            protected override void OnRegistration(StackEx<SpanSyntaxNode> stack)
            {
                if(stack.Count == 1)
                    is_first = true;                
            }

            public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
            {
                if (index.Value is EndOfline)
                    is_last = true;

                return base.Parse (ref index);
            }

            public override string GenerateCode()
            {
                string style = "";

                if (is_first && !is_last)
                    style = " style=\"float:left;\"";

                if (is_last && !is_first)
                    style = " style=\"float:right;\"";
                
                var id = this.element_id != null ? " id=\"" + this.element_id + "\"" : null;
                var title = this.title != null ? " title=\"" + this.title + "\"" : null;
                var path = this.path != null ? " src=\"" + this.path + "\"" : null;

                if (id == null && title == null && path == null)
                    return "<!-- Error Processing Image Directive -->";

                return string.Format ("<img" + " {0}".TrimStart (1) + " {1}".TrimStart (1) + " {2}".TrimStart (1) + "{3}".TrimStart(1) + " />", id, title, path, style);
            }
        }

        sealed class StrikeExpression : TerminalSpan
        {
            TerminalSpan exp;
            
            static bool GoodState(StackEx<SpanSyntaxNode> stack)
            {
                var top = stack.TopNode;

                while (top != null)
                {
                    if(top.Value == null)
                        return false;

                    top = top.Next;
                }

                return true;
            }

            protected override void OnRegistration(StackEx<SpanSyntaxNode> stack)
            {
                if (parentStack.Count > 1)
                {
                    var neighbor = parentStack.TopNode.Next;

                    while (neighbor.Value is PartialWhitespaceExpression.WhitespaceExpression && neighbor.Next != null)
                        neighbor = neighbor.Next;

                    if (neighbor != null && neighbor.Value != null)
                    {
                        parentStack.Pop ();
                        
                        exp = neighbor.Value as TerminalSpan;

                        neighbor.Value = this;
                    }                    
                }
            }

            public override string GenerateCode()
            {
                return "<s>" + (exp != null ? exp.GenerateCode () : "") + "</s>";
            }
        }

        sealed class UnknownInlineDirective : TerminalSpan
        {
            public override string GenerateCode()
            {
                return "<!-- Unknown Directive -->";
            }
        }

        sealed class CorrectionExpression : TerminalSpan
        {
            TerminalSpan exp;
            readonly string text;

            public CorrectionExpression(string text)
            {
                this.text = text;
            }
            
            protected override void OnRegistration(StackEx<SpanSyntaxNode> stack)
            {
                if (stack.Count > 1)
                {
                    var neighbor = stack.TopNode.Next;

                    int whitespace_count = 0;

                    while (neighbor.Value is PartialWhitespaceExpression.WhitespaceExpression && neighbor.Next != null)
                    {
                        whitespace_count++;
                        neighbor = neighbor.Next;
                    }

                    if (neighbor != null && (exp = neighbor.Value as TerminalSpan) != null)
                    {
                        parentStack.Pop ();

                        for (var i = 0; i < whitespace_count; i++)
                            parentStack.Pop ();

                        neighbor.Value = this;
                    }
                }
            }

            public override string GenerateCode()
            {
                return "<del>" + exp.GenerateCode () + "</del> <ins>" + new ParsedTextRun (text.TrimStart(1)).GenerateCode () + "</ins>";
            }
        }
    }
}