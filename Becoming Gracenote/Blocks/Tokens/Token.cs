namespace Gracenote
{
    using System.Linq;
    using System;
    using System.Collections.Generic;

    public abstract class ListStyleType
    {
        public abstract string GetStyle();

        static readonly ListStyleType digit          = new Ordered.Digit ();
        static readonly ListStyleType leading_zero   = new Ordered.DigitLeadingZero ();
        static readonly ListStyleType lower_alpha    = new Ordered.LowerAlpha ();
        static readonly ListStyleType upper_alpha    = new Ordered.UpperAlpha ();
        static readonly ListStyleType lower_roman    = new Ordered.LowerRoman ();
        static readonly ListStyleType upper_roman    = new Ordered.UpperRoman ();
        static readonly ListStyleType bulleted       = new Unordered.Bulleted ();


        public static ListStyleType GetStyleType(char c)
        {
            if (char.IsDigit (c))
            {
                if (c == '0')
                    return leading_zero;
                else
                    return digit;
            }

            if (char.IsLetter (c))
            {
                if (char.IsLower (c))
                {
                    if (c == 'i')
                        return lower_roman;
                    else
                        return lower_alpha;
                }
                else
                {
                    if (c == 'I')
                        return upper_roman;
                    else
                        return upper_alpha;
                }
            }

            return bulleted;
        }
        
        public abstract class Ordered : ListStyleType
        {
            public sealed class Digit : Ordered
            {
                public override string GetStyle()
                {
                    return "decimal";
                }
            }

            public sealed class DigitLeadingZero : ListStyleType
            {
                public override string GetStyle()
                {
                    return "decimal-leading-zero";
                }
            }

            public sealed class LowerAlpha : Ordered
            {
                public override string GetStyle()
                {
                    return "lower-alpha";
                }
            }

            public sealed class UpperAlpha : Ordered
            {
                public override string GetStyle()
                {
                    return "upper-alpha";
                }
            }

            public sealed class LowerRoman : Ordered
            {
                public override string GetStyle()
                {
                    return "lower-roman";
                }
            }

            public sealed class UpperRoman : Ordered
            {
                public override string GetStyle()
                {
                    return "upper-roman";
                }
            }
        }

        public abstract class Unordered : ListStyleType
        {
            public sealed class Bulleted : Unordered
            {
                public override string GetStyle()
                {
                    return "disc";
                }
            }
        }
    }
    
    public abstract class LineToken : Identity, StackEx<LineToken>.Membership
    {
        static char[] hr_chars = new[] { '-', '=', '_', '+', '~' };

        public static readonly string TEXT_FRAGMENT_SIGNAL = Guid.NewGuid ().ToString();
        
        protected internal string line;

        readonly Guid id = Guid.NewGuid ();

        public Guid ID
        {
            get
            {
                return id;
            }
        }

        StackEx<LineToken> parentStack;

        public void Register(StackEx<LineToken> stack)
        {
            this.parentStack = stack;
        }

        public void Deregister(StackEx<LineToken> stack)
        {
            this.parentStack = null;    
        }

        public LineToken(string line)
        {
            this.line = line.Replace("\t", "    "); //this doesn't mean war.
        }

        public int LeadingWhiteSpaceCount
        {
            get
            {
                return line.GetLeadingWhiteSpaceCount ();
            }
        }

        public virtual LineToken Lex(string line, ref int i)
        {
            return Start (line, ref i);
        }

        public static LineToken Start(string line, ref int i)
        {
            if (IsBlank(line))
                return new Blank (line);            
            
            var match = Utils.ListPattern.Match (line);

            if (match.Success)
            {
                var leading = match.Groups["leading"].Value.Length;
                var trailing = match.Groups["trailing"].Value.Length;
                var label = match.Groups["digit"].Value.Length + match.Groups["upper"].Value.Length + match.Groups["lower"].Value.Length;
                
                return new ListToken (line, leading + label + trailing, ListStyleType.GetStyleType(line.Trim()[0]));
            }

            match = Utils.QuotePattern.Match (line);

            if (match.Success)
                return new BlockQuote (line);

            

            if (line.StartsWith ("[") && line.EndsWith ("]") && line.Substring (1, line.Length - 2).All (c => c != ']' && c != '['))
                return new Directive (line);


            match = Utils.ReferencePattern.Match (line);


            System.Diagnostics.Trace.WriteLineIf (line.StartsWith ("[2]"), match.Success);

            if (match.Success)
                return ReferenceItemToken.Create (match.Groups["refname"].Value, line);


            foreach (var c in hr_chars)
                if (line.Length >= 3 && line.All (x => x == c))
                    return HorizontalRule.Create (line);

            //foreach (var c in hr_chars)
            //        if (line.StartsWith (c.ToString(), StringComparison.Ordinal) && line.Replace (c.ToString (), "").Length > 0)
            //            return new Header (line);

            if (line.StartsWith (TEXT_FRAGMENT_SIGNAL))
                return new TextFragmentToken (line.Substring (TEXT_FRAGMENT_SIGNAL.Length));

            return new Paragraph (line);
        }

        public abstract SyntaxNode StartNode(ref int i);

        protected static bool IsBlank(string line)
        {
            return line.Trim ().Length == 0;
        }

        public virtual int NumberOfBlankLines
        {
            get
            {
                return 0;
            }
        }

        public static implicit operator string(LineToken token)
        {
            return token.line;
        }
    }

    public sealed class Tokenized : IEnumerable<LineToken>
    {
        readonly IEnumerable<LineToken> tokens; 
        
        public Tokenized(string text) :this(text != null ? text.Split (new[] { "\r" }, StringSplitOptions.None) : null)
        {
            
        }

        public Tokenized(IEnumerable<string> lines)
        {
            if (lines.Any ())
            {
                var line_array = lines.ToArray ();

                var i = 0;

                var stack = new StackEx<LineToken> (LineToken.Start (line_array[0], ref i));

                while (++i < line_array.Length)
                    stack.Push (stack.Top.Lex (line_array[i], ref i));

                var refs = new List<ReferenceItemToken> ();

                foreach (var token in stack)
                {
                    var ref_item = token as ReferenceItemToken;

                    if (ref_item != null)
                        refs.Add (ref_item);
                }

                System.Diagnostics.Trace.WriteLine (refs.Count);

                foreach (var token in stack)
                {
                    if (token is ReferenceItemToken)
                        continue;
                    else
                        foreach (var ref_itme in refs)
                        {
                            if(token.line.Contains('[' + ref_itme.name + ']'))                            
                                token.line = ref_itme.ReplaceReferences (token.line);
                        }
                }
    
                tokens = stack.Reverse ();
            }
            else
            {
                tokens = Enumerable.Empty<LineToken> ();
            }
        }

        public IEnumerator<LineToken> GetEnumerator()
        {
            return tokens.GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }
    }
}

