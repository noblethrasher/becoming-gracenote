using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Text.RegularExpressions;

namespace Gracenote
{
    public static class DirectiveExpressionModule
    {
        static Regex grid_pattern = new Regex (@"^grid\s?\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public static SyntaxNode CreateDirective(string line, int line_number)
        {
            var LINE = line.ToUpper ();

            if (LINE.StartsWith ("FIG:") || LINE.StartsWith ("FIGURE:") || LINE.StartsWith("PIC:") || LINE.StartsWith("PICTURE:") || LINE.StartsWith("IMG:") || LINE.StartsWith("IMAGE:"))
                return new BlockImage (line, line_number);

            
            
            if (LINE.Length > 5 &&LINE.StartsWith ("CLIP:"))
            {
                var data = line.Substring (line.IndexOf (':') + 1);

                return MediaClipExpression.Create (data);
            }


            if (grid_pattern.IsMatch (LINE))
            {
                return new PartialGrid (LINE.Substring (4), LINE.GetLeadingWhiteSpaceCount());
            }

            
            
            switch (LINE)
            {
                case "ADDRESS":
                    return new PartialAddressExpression ();

                case "CODE":
                    return new PartialCodeExpression ();

                case "COLUMN":
                    return new PartialColumn ();
                
                default:
                    return new UnknownDirectiveExpression (line, line_number);
            }
        }

        public static SyntaxNode CreateDirective(this Directive dir, int line_number)
        {
            return CreateDirective ((string)dir, line_number);
        }

        abstract class MediaClipExpression : TerminalLine
        {
            public static MediaClipExpression Create(string data)
            {
                var uri_xs = Utils.URI_Pattern.Matches (data);

                if (uri_xs.Count > 0)
                {
                    data = Utils.URI_Pattern.Replace (data, "");

                    var url = new Uri (uri_xs[0].Value);

                    var host = url.Host.ToUpper ();

                    switch (host)
                    {
                        case "YOUTUBE.COM":
                        case "WWW.YOUTUBE.COM":
                        case "YOUTU.BE":
                            return new YouTubeClip (url, data);

                        default:
                            return null;
                    }
                }
                else
                {
                    return null;
                }

            }

            sealed class YouTubeClip : MediaClipExpression
            {
                static Regex data_parser = new Regex (@"width=(?'width'\d+(\.\d+)?)|height=(?'height'\d+(\.\d+)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                
                string video_id;
                string data;

                public bool AutoPlay { get; private set; }

                List<string> width, height;
                
                public YouTubeClip(Uri uri, string data)
                {
                    this.data = data ?? "";

                    AutoPlay = data.Contains ("autoplay") || data.Contains ("autostart") || data.Contains ("auto play") || data.Contains ("auto start");

                    var matches = data_parser.Matches (data);

                    foreach (Match match in matches)
                    {
                        if (match.Value.StartsWith ("width", StringComparison.OrdinalIgnoreCase))
                        {
                            (width = width ?? new List<string> ()).Add (match.Groups["width"].Value);
                            continue;
                        }

                        if (match.Value.StartsWith ("height", StringComparison.OrdinalIgnoreCase))
                        {
                            (height = height ?? new List<string> ()).Add (match.Groups["height"].Value);
                            continue;
                        }
                    }

                    if (uri.Host == "youtu.be")
                    {
                        if (uri.Segments.Length > 1)
                            video_id = uri.Segments[1];
                    }
                    else

                    if (uri.Query.Length > 2)
                    {
                        var qs = uri.Query.Substring(1).Split ('&');

                        for (var i = 0; i < qs.Length; i++)
                        {
                            if (qs[i].StartsWith ("v=") && qs[i].Length > 2)
                            {
                                var temp = qs[i].Split ('=')[1];

                                for (var j = 0; j < temp.Length; j++)
                                    if (char.IsLetterOrDigit (temp[j]) || temp[j] == '-' || temp[j] == '_' || temp[j] == '+')
                                        continue;
                                    else
                                        goto STOP;

                                video_id = temp;
                                break;
                            }

                        STOP:
                            continue;
                        }
                    }
                }

                public override string GenerateCode(uint n = 0)
                {
                    var sb = new StringBuilder ();

                    sb.AppendLine ("<div class=\"ytclip\">", n);

                    sb.AppendLine ("<iframe id=\"ytplayer\" type=\"text/html\" " + (string.Join (" ", Width, Height) + " ") + "src=\"http://www.youtube.com/embed/" + video_id + "?autoplay=" + (AutoPlay ? "1" : "0") + "&origin=http://example.com\" frameborder=\"0\"></iframe>", n + 1);

                    sb.AppendLine ("</div>");

                    return sb.ToString ();
                }


                private static string ComputeDimensionAttribute(string attr_name, IEnumerable<string> values)
                {
                    if (values != null && values.Any ())
                    {
                        double k = 1;

                        foreach (var y in values)
                        {
                            var xs = y.Split ('.');

                            var whole = double.Parse (new string (xs[0].Take (6).ToArray ()));
                            var fractional = 0.0;

                            if (xs.Length > 1)
                                fractional = double.Parse ("0." + xs[1]);

                            k *= (whole + fractional);                            
                        }

                        return attr_name + "=\"" + Math.Round(k, 0) + "\"";
                    }

                    return null;
                }

                private string Height
                {
                    get
                    {
                        return ComputeDimensionAttribute ("height", height);
                    }
                }

                private string Width
                {
                    get
                    {
                        return ComputeDimensionAttribute ("width", width);
                    }
                }
            }
        }

        sealed class PartialAddressExpression : NonTerminalLine
        {
            public List<string> lines = new List<string> ();

            public override SyntaxNode Parse(LineToken token, ref int i)
            {
                if (token.NumberOfBlankLines > 1 || token is EndOfFileToken || (token is Directive && ((string)token).StartsWith("END", System.StringComparison.OrdinalIgnoreCase) ))
                    return Complete (token, ref i);
                else
                {
                    lines.Add (LineToken.TEXT_FRAGMENT_SIGNAL + token);
                    return this;
                }
            }

            public SyntaxNode Complete(LineToken token, ref int i)
            {
                parentStack.Pop ();

                parentStack.Push (new CompleteAddress (this));

                return token.StartNode (ref i);
            }

            sealed class CompleteAddress : TerminalLine
            {
                readonly PartialAddressExpression addr;

                public CompleteAddress(PartialAddressExpression addr)
                {
                    this.addr = addr;
                }

                public override string GenerateCode(uint n = 0)
                {
                    var sb = new StringBuilder ();

                    var parsed = new Parsed (addr.lines);

                    foreach (var exp in parsed)
                    {
                        sb.AppendLine ("<address>", n);
                        sb.AppendLine (exp.GenerateCode (n + 1).TrimEnd ());
                        sb.AppendLine ("</address>", n);
                    }                    

                    return sb.ToString ();
                }
            }
        }

        sealed class UnknownDirectiveExpression : TerminalLine
        {
            readonly string line;
            readonly int line_number;

            public UnknownDirectiveExpression(string line, int line_number)
            {
                this.line = line;
                this.line_number = line_number;
            }
            
            public override string GenerateCode(uint n = 0)
            {
                return '\t'.Repeat(n) + string.Format("<!-- Unknown Directive '{0}' at line {1}  -->", line, line_number);
            }
        }

        sealed class PartialCodeExpression : NonTerminalLine
        {
            public List<string> lines = new List<string> ();
            
            public override SyntaxNode Parse(LineToken token, ref int i)
            {
                bool end_directive = false;
                
                if (token is EndOfFileToken || (token is Directive && (end_directive = ((string)token).StartsWith ("END", System.StringComparison.OrdinalIgnoreCase))))
                {
                    if (end_directive)
                        lines.Add (token);
                    
                    return new CompleteCode (this);
                }
                else
                {
                    lines.Add (token);
                    return this;
                }
            }

            public SyntaxNode Complete(LineToken token, ref int i)
            {
                parentStack.Pop ();

                parentStack.Push (new CompleteCode (this));

                return token.StartNode (ref i);
            }

            sealed class CompleteCode : TerminalLine
            {
                readonly IEnumerable<string> lines;

                public CompleteCode(PartialCodeExpression exp)
                {
                    lines = exp.lines;
                }

                public override string GenerateCode(uint n = 0)
                {
                    var sb = new StringBuilder ();

                    sb.AppendLine ("<code>", n);

                    foreach (var line in lines)
                    {
                        var line_ = line.Replace (" ", "&nbsp;").Replace ("<", "&lt;").Replace (">", "&gt;") + "<br />";

                        sb.AppendLine (line_, n + 1);
                    }

                    sb.AppendLine ("</code>", n);

                    return sb.ToString ();
                }
            }
        }

        sealed class BlockImage : TerminalLine
        {
            readonly string id, path, title;
            readonly int line_number;

            public BlockImage(string line, int line_number)
            {

                var index = line.IndexOf (':');

                var data = "";

                if (line.Length - 1 > index)
                    data = line.Substring (index + 1);

                var result = ImageDataParser.Parse (data);

                id = result.ID;
                title = result.Title;
                path = result.Path;

                this.line_number = line_number;
            }

            public override string GenerateCode(uint n = 0)
            {
                var id = this.id != null ? " id=\"" + this.id + "\"" : null;
                var title = this.title != null ? " title=\"" + this.title + "\"" : null;
                var path = this.path != null ? " src=\"" + Parsed.RelativeMediaPath + this.path + "\"" : null;

                if (id == null && title == null && path == null)
                    return "<!-- Error Processing Image Directive at line " + line_number + " -->";

                var sb = new StringBuilder ();

                sb.AppendLine ("<div class=\"imgblock\">", n);

                sb.AppendLine (string.Format ("<img" + " {0}".TrimStart (1) + " {1}".TrimStart (1) + " {2}".TrimStart (1) + " />", id, title, path), n + 1);

                sb.AppendLine ("</div>", n);

                return sb.ToString ();
            }
        }

        sealed class PartialGrid : NonTerminalLine
        {
            public int cols;
            public int leading;

            public readonly List<LineToken> lines = new List<LineToken> ();

            static char[] separators = new[] { ' ' };

            public PartialGrid(string s, int leading)
            {
                this.leading = leading;

                var @params = s.Split (separators, StringSplitOptions.RemoveEmptyEntries);

                cols = ParseThreeDigitInt (@params[0]);
            }

            static int ParseThreeDigitInt(string s)
            {
                var len = Math.Min (3, s.Length);

                int result = 0;

                for (var i = 0; i < len; i++)
                    result += (int)Math.Pow (10, i) * (s[i] - 48);

                return result;
            }

            public override SyntaxNode Parse(LineToken token, ref int i)
            {
                if (token is Directive)
                    if (token.LeadingWhiteSpaceCount == leading)
                        if (token.line.ToUpper().Contains ("END GRID"))
                            return new Grid (this);

                if (token is EndOfFileToken)
                    return new Grid (this);

                if (token.NumberOfBlankLines > 2)
                    return new Grid (this);


                lines.Add (token);

                return this;
            }
        }
        
        sealed class Grid : TerminalLine
        {
            PartialGrid pg;
            
            public Grid(PartialGrid pg)
            {
                this.pg = pg;
            }

            public override string GenerateCode(uint n = 0)
            {                
                var stack = new Stack<List<TerminalLine>> ();

                var i = 0;

                foreach (var terminal in new Parsed (pg.lines))
                {
                    if (terminal is BlankLineExpression)
                        continue;
                    
                    if ((i % pg.cols) == 0)
                        stack.Push (new List<TerminalLine> ());

                    stack.Peek ().Add (terminal);
                    i++;
                }

                var rows = stack.Reverse ();

                var sb = new StringBuilder ();

                sb.AppendLine("<table class=\"gn_grid\">", n);

                
                int k = rows.Count();

                foreach (var row in rows)
                {
                    sb.AppendLine ("<tr>", n + 1);

                    int j = 0;

                    foreach (var terminal in row)
                    {
                        sb.AppendLine ("<td class=\"c" + (j % k) + "\">", n + 2);
                        sb.Append (terminal.GenerateCode(n + 3));
                        sb.AppendLine ("</td>", n + 2);

                        j++;
                    }

                    sb.AppendLine ("</tr>", n + 1);
                }

                sb.AppendLine ("</table>", n);

                return sb.ToString ();    
            }
        }
    }

    sealed class PartialColumn : NonTerminalLine
    {
        readonly List<LineToken> tokens = new List<LineToken> ();
        
        public override SyntaxNode Parse(LineToken token, ref int i)
        {
            LineToken directive = null;            
            
            if(token.NumberOfBlankLines > 2 || token is EndOfFileToken || ((directive = token as Directive) != null) && token.line.Equals("Column", StringComparison.OrdinalIgnoreCase))
            {
                parentStack.Pop ();

                parentStack.Push (new CompleteColumn (this));

                return token.StartNode (ref i);
            }
            
            tokens.Add (token);

            return this;
        }

        sealed class CompleteColumn : TerminalLine
        {
            readonly PartialColumn pt;

            public CompleteColumn(PartialColumn pt)
            {
                this.pt = pt;
            }

            protected override void OnRegistration(StackEx<SyntaxNode> stack)
            {
                if (stack.Count > 1)
                {
                    var completeColumn = stack.TopNode.Next.Value as CompleteColumn;

                    if (completeColumn != null)
                    {
                        parentStack.Pop ();
                        parentStack.Pop ();

                        parentStack.Push (new MultiColumn (completeColumn, this));
                        return;
                    }

                    var multi = stack.TopNode.Next.Value as MultiColumn;

                    if (multi != null)
                    {
                        parentStack.Pop ();

                        multi.Add (this);

                        return;
                    }
                }
            }

            public override string GenerateCode(uint n = 0)
            {
                var sb = new StringBuilder ();

                sb.AppendLine ("<table class=\"gn_columns\">" , n);
                sb.AppendLine ("<tr>", n + 1);

                sb.AppendLine ("<td>", n + 2);

                sb.AppendLine (new Parsed (pt.tokens).GenerateCode (), n + 3);


                sb.AppendLine ("</td>", n + 2);

                sb.AppendLine ("</tr>", n + 1);
                sb.AppendLine ("</table>", n);

                return sb.ToString ();
            }

            sealed class MultiColumn : TerminalLine
            {
                
                
                
                readonly List<CompleteColumn> columns = new List<CompleteColumn> ();

                public MultiColumn(CompleteColumn left, CompleteColumn right)
                {
                    columns.Add (left);
                    columns.Add (right);
                }

                public void Add(CompleteColumn column)
                {
                    columns.Add (column);
                }

                public override string GenerateCode(uint n = 0)
                {
                    var sb = new StringBuilder ();

                    sb.AppendLine ("<table class=\"gn_columns\">", n);

                    sb.AppendLine ("<tr>", n + 1);

                    foreach (var column in columns)
                    {
                        sb.AppendLine ("<td>", n + 2);

                        sb.AppendLine (column.GenerateCode(n + 3));

                        sb.AppendLine ("</td>", n + 2);
                    }

                    sb.AppendLine ("</tr>", n + 1);

                    sb.AppendLine ("</table>", n);

                    return sb.ToString ();
                }
            }
        }
    }
}