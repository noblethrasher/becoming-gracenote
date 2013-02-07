using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gracenote
{
    class GuardAttribute : Attribute
    {
        public readonly char[] options;
        public string ErrorMessage { get; set; }
        
        public GuardAttribute(params char[] options)
        {
            this.options = options;
        }

        public bool Test(char c)
        {
            return options.Any (x => x == c);
        }

        public bool Test(params char[] cs)
        {
            return cs.All (x => options.Any (c => c == x));       
        }
    }
    
    public abstract class SpanToken : Identity, StackEx<SpanToken>.Membership
    {
        protected readonly List<char> chars = new List<char>();        

        Guid id = Guid.NewGuid ();

        public Guid ID
        {
            get
            {
                return id;
            }
        }

        public virtual int WhitesSpaceLength
        {
            get
            {
                return 0;
            }
        }

        public virtual bool ContainsNewLines
        {
            get
            {
                return false;
            }
        }

        //public SpanToken(params char[] cs)
        //{
        //    var guard = this.GetType ().GetCustomAttributes (false).Select (x => x as GuardAttribute).FirstOrDefault ();

        //    if(guard != null)
        //        if(!guard.Test(cs))
        //            throw new ArgumentException (guard.ErrorMessage ?? "Invalid character '" + c + "' passed to Constructor of " + this.GetType ().Name + (guard.options.Length > 1 ? " Exptected '" + guard.options + "'" : "Expected one of the following: " + string.Join (",", guard.options)));

        //    chars.AddRange (cs);
        //}

        public SpanToken()
        {

        }

        public SpanToken(char c)
        {
            var guard = this.GetType ().GetCustomAttributes (false).Select (x => x as GuardAttribute).FirstOrDefault ();

            if (guard != null)
                if (!guard.Test (c))
                    throw new ArgumentException (guard.ErrorMessage ?? "Invalid character '" + c + "' passed to Constructor of " + this.GetType ().Name + (guard.options.Length > 1 ? " Exptected '" + guard.options + "'" : "Expected one of the following: " + string.Join(",", guard.options) ));
            
            chars.Add (c);
        }

        public string GetString()
        {
            return new string (chars.ToArray ());
        }

        protected void Add(char c)
        {
            chars.Add (c);
        }

        public virtual SpanToken Scan(ref Indexical<CharEx> index)
        {
            return Start (ref index);
        }

        public abstract SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index);
        
        public static SpanToken Start(ref Indexical<CharEx> index)
        {

            SpanToken token = null;

            if ((token = TokenFromSigil<UriToken> (TokenizedSpans.url_sigil, ref index)) != null)
                return token;

            if ((token = TokenFromSigil<NakedUrlToken> (TokenizedSpans.naked_url_sigil, ref index)) != null)
                return token;

            if ((token = TokenFromSigil<TimeToken> (TokenizedSpans.time_sigil, ref index)) != null)
                return token;

            if ((token = TokenFromSigil<InlineDirectiveToken> (TokenizedSpans.inline_directive_sigil, ref index)) != null)
                return token;

            if ((token = TokenFromSigil<EmptyImage> (TokenizedSpans.empty_image_sigil, ref index)) != null)
                return token;

            if ((token = TokenFromSigil<Trademark> (TokenizedSpans.trademark_sigil, ref index)) != null)
                return token;
            
            if ((token = TokenFromSigil<RegisteredTrademark> (TokenizedSpans.registered_trademark_sigil, ref index)) != null)
                return token;

            if ((token = TokenFromSigil<Copyright> (TokenizedSpans.copyright_sigil, ref index)) != null)
                return token;




            //if (c == TokenizedSpans.url_sigil[0])
            //{
            //    if (index + (TokenizedSpans.url_sigil.Length - 1))
            //    {
            //        var i = 1;

            //        var upper = index;

            //        while ((++upper).IsValid && i < TokenizedSpans.url_sigil.Length)
            //            if (upper.Value != TokenizedSpans.url_sigil[i])
            //                break;
            //            else
            //                i++;

            //        if (i == TokenizedSpans.url_sigil.Length)
            //        {
            //            index = index + TokenizedSpans.url_sigil.Length - 1;
            //            return new UriToken ();  
            //        }
            //    }
            //}

            //if (c == TokenizedSpans.naked_url_sigil[0])
            //{
            //    if (index + (TokenizedSpans.naked_url_sigil.Length - 1))
            //    {
            //        var i = 1;

            //        var upper = index;

            //        while ((++upper).IsValid && i < TokenizedSpans.naked_url_sigil.Length)
            //            if (upper.Value != TokenizedSpans.naked_url_sigil[i])
            //                break;
            //            else
            //                i++;

            //        if (i == TokenizedSpans.naked_url_sigil.Length)
            //        {
            //            index = index + TokenizedSpans.naked_url_sigil.Length - 1;
            //            return new NakedUrlToken ();
            //        }
            //    }
            //}

            //if (c == TokenizedSpans.time_sigil[0])
            //{
            //    if (index + (TokenizedSpans.time_sigil.Length - 1))
            //    {
            //        var i = 1;

            //        var upper = index;

            //        while ((++upper).IsValid && i < TokenizedSpans.time_sigil.Length)
            //            if (upper.Value != TokenizedSpans.time_sigil[i])
            //                break;
            //            else
            //                i++;

            //        if (i == TokenizedSpans.time_sigil.Length)
            //        {
            //            index = index + TokenizedSpans.time_sigil.Length - 1;
            //            return new TimeToken ();
            //        }
            //    }
            //}

            //if (c == TokenizedSpans.inline_directive_sigil[0])
            //{
            //    if (index + (TokenizedSpans.inline_directive_sigil.Length - 1))
            //    {
            //        var i = 1;

            //        var upper = index;

            //        while ((++upper).IsValid && i < TokenizedSpans.inline_directive_sigil.Length)
            //            if (upper.Value != TokenizedSpans.inline_directive_sigil[i])
            //                break;
            //            else
            //                i++;

            //        if (i == TokenizedSpans.inline_directive_sigil.Length)
            //        {
            //            index = index + TokenizedSpans.inline_directive_sigil.Length - 1;
            //            return new InlineDirectiveToken ();
            //        }
            //    }
            //}

            
            //if (c == TokenizedSpans.empty_image_sigil[0])
            //{
            //    if (index + (TokenizedSpans.empty_image_sigil.Length - 1))
            //    {
            //        var upper = index;

            //        int i = 1;

            //        while ((++upper).IsValid && i < TokenizedSpans.empty_image_sigil.Length)
            //        {
            //            if (upper.Value != TokenizedSpans.empty_image_sigil[i])
            //                break;
            //            else
            //                i++;
            //        }

            //        if (i == TokenizedSpans.empty_image_sigil.Length)
            //        {

            //            index = index + TokenizedSpans.empty_image_sigil.Length - 1;

            //            return new EmptyImage ();
            //        }
            //    }
            //}

            

            var c = index.Value;

            if (c == TokenizedSpans.email_sigil[0])
            {
                if (index + (TokenizedSpans.email_sigil.Length - 1))
                {
                    var i = 1;

                    var upper = index;

                    while ((++upper).IsValid && i < TokenizedSpans.email_sigil.Length)
                        if (upper.Value != TokenizedSpans.email_sigil[i])
                            break;
                        else
                            i++;

                    if (i == TokenizedSpans.email_sigil.Length)
                    {
                        index = index + TokenizedSpans.time_sigil.Length - 1;

                        var num = new char[3];

                        for (var k = 0; k <= 2; k++)
                        {
                            num[k] = (++index).Value;
                        }

                        return new Email (int.Parse (new string (num)));
                    }
                }
            }



            if (c.IsDigit)
                return new Digits (c);

            if (c.IsLetter)
                return new WordToken (c);

            if (c.IsCaret)
                return new Caret (c);

            if (c.IsDoubleQuote)
                return new DoubleQuote (c);

            if (c.IsGraveAccent)
                return new GraveAccent (c);

            if (c.IsLeftBracket)
                return new LeftBraket (c);

            if (c.IsRightBracket)
                return new RightBraket (c);

            if (c.IsLeftParen)
                return new LeftParen (c);

            if (c.IsRightParen)
                return new RightParen (c);

            if (c.IsWhiteSpace)
                return new WhiteSpace (c);

            if (c == ',')
                return new Comma (c);

            if (c == '\\')
                return new PartialLiteralToken (c);

            if (c == '*')
                return new Asterisk (c);

            if (c == '_')
                return new Underscore (c);

            if (c == '-')
                return new Dash (c);

            if (c == '.')
                return new Dot (c);

            if (c == '#')
                return new Hash (c);

            if (c == ';')
                return new Semicolon (c);

            if (c == '&')
                return new Ampersand (c);

            if (c == '+')
            {
                var max_reach = index + 2;

                if (max_reach)
                {
                    var next = index + 1;

                    if (next.Value == '/' && max_reach.Value == '-')
                    {                        
                        index = index + 2;
                        return new PlusMinus (c, next.Value, max_reach.Value);
                    }
                }
            }

            if (c == '!')
            {
                var next = index + 1;

                if (next.IsValid && next.Value == '=')
                {
                    index = index + 1;
                    return new NotEqual (c, next.Value);
                }
            }

            if (c == '~')
            {
                var next = index + 1;

                if (next.IsValid && next.Value == '=')
                {
                    index = index + 1;
                    return new AlmostEqual (c, next.Value);
                }
            }

            if (c == '<')
            {
                var next = index + 1;

                if (next.IsValid && next.Value == '=')
                {
                    index = index + 1;
                    return new LessThanOrEqual (c, next.Value);
                }
            }

            if (c == '>')
            {
                var next = index + 1;

                if (next.IsValid && next.Value == '=')
                {
                    index = index + 1;
                    return new GreaterThanOrEqual (c, next.Value);
                }
            }

            if (c == '\'')
                return new SingleQuote (c);

            if (char.IsPunctuation (c))
                return new GenericPunctuation (c);

            if (char.IsSymbol (c))
                return new GenericSymbol (c);

            return new UncategorizedToken (c);
        }

        protected StackEx<SpanToken> parentStack;

        public void Register(StackEx<SpanToken> stack)
        {
            parentStack = stack;
        }

        public void Deregister(StackEx<SpanToken> stack)
        {
            return;
        }

        public override string ToString()
        {
            return GetType ().Name + " : " + GetString ();
        }

        static SpanToken TokenFromSigil<T>(string sigil, ref Indexical<CharEx> index, bool consume_last = false)
            where T : SpanToken, new()
        {
            var c = index.Value;
            
            if (c == sigil[0])
            {
                if (index + (sigil.Length - 1))
                {
                    var i = 1;

                    var upper = index;

                    while ((++upper).IsValid && i < sigil.Length)
                        if (upper.Value != sigil[i])
                            break;
                        else
                            i++;

                    if (i == sigil.Length)
                    {
                        index = index + sigil.Length - 1;
                        
                        return new T ();
                    }                    
                }
            }

            return null;
        }
    }

    public class TokenizedSpans : IEnumerable<SpanToken>
    {
        IEnumerable<SpanToken> tokens;

        static readonly Regex url_pattern = new Regex (@"(?'paren'\((?'uri'(?'url_part'(?'scheme'(http(s)?|(s)?ftp|about|))://(?'host'(\w|\.|@)+(:\d+)?)(?'path'/([a-z]|[A-Z]|[0-9]|#|\(|\)|\+|%(([a-f]|[A-F])|\d){2}|~|\.|/|_|-|$|@|;|:|'|!|=|\?|&)*)?)(\s+""(?'titletext'.*)"")?)\))|(?'naked'(?'url_part'(?'scheme'(http(s)?|(s)?ftp|about|))://(?'host'(\w|\.|@)+(:\d+)?)(?'path'/(#|[a-z]|[A-Z]|[0-9]|\(|\)|\+|%(([a-f]|[A-F])|\d){2}|~|\.|/|_|-|$|@|;|:|'|!|=|\?|&)*)?))", RegexOptions.Compiled);
        static readonly Regex time_pattern = new Regex (@"(?'time'(?'hour'\d\d?):(?'minute'\d\d))\s?(?'period'(([aA]|[pP])[mM])|(([aA]|[pP])\.[mM]\.))", RegexOptions.Compiled);
        static readonly Regex inline_directive_pattern = new Regex (@"(?<!\\)\[(?'directive'img|image|fig|figure|picture|pic|clip|correction):(?'data'[^\]]+)(?<!\\)\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex strike_directive = new Regex(@"(?<!\\)\[strike\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex empty_image = new Regex (@"(?<!\\)\[placeholder\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex commercial_symbols = new Regex (@"(?'head'\s|^)\((?'type'tm|r|c)\)(?'tail'.?)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static readonly string url_sigil;
        public static readonly string naked_url_sigil;
        public static readonly string time_sigil;
        public static readonly string email_sigil;
        public static readonly string inline_directive_sigil;
        public static readonly string empty_image_sigil;
        public static readonly string trademark_sigil;
        public static readonly string registered_trademark_sigil;
        public static readonly string copyright_sigil;

        static TokenizedSpans()
        {
            var bytes = new byte[50];

            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider ();
            
            rng.GetBytes (bytes);

            url_sigil = Convert.ToBase64String (bytes);


            rng.GetBytes (bytes);

            naked_url_sigil = Convert.ToBase64String (bytes);


            rng.GetBytes (bytes);

            time_sigil = Convert.ToBase64String (bytes);


            rng.GetBytes (bytes);

            email_sigil = Convert.ToBase64String (bytes);


            rng.GetBytes (bytes);

            inline_directive_sigil = Convert.ToBase64String (bytes);

            rng.GetBytes (bytes);

            empty_image_sigil = Convert.ToBase64String (bytes);

            rng.GetBytes (bytes);

            trademark_sigil = Convert.ToBase64String (bytes);

            rng.GetBytes (bytes);

            registered_trademark_sigil = Convert.ToBase64String (bytes);

            rng.GetBytes (bytes);

            copyright_sigil = Convert.ToBase64String (bytes);
        }

        static string OnEmptyImageMatch(Match match)
        {
            return empty_image_sigil;
        }

        static string OnCommercialSymbol(Match match)
        {
            var tail = match.Groups["tail"].Value;
            var head = match.Groups["head"].Value;

            if (tail.Length > 0 && char.IsLetter (tail[0]))
                return match.Value;

            switch(match.Groups["type"].Value.ToUpper())
            {
                case "TM":
                    return head + trademark_sigil + tail;
                case "R":
                    return head + registered_trademark_sigil + tail;
                case "C":
                    return head + copyright_sigil + tail;

                default:
                    throw new Exception ("Unknown Sigil: '" + match.Groups["type"].Value.ToUpper());
            }
        }

        static string OnUrlMatch(Match match)
        {            
            var paren = match.Groups["paren"];
            var naked = match.Groups["naked"];

            if (paren.Length > 0)
            {
                return url_sigil + match.Groups["url_part"].Value + ">" + match.Groups["titletext"].Value + "\\ "; //return value must have a trailing space or else the directive regex will not detect directives that contain only a url
            }
            else
            {
                return naked_url_sigil + match.Groups["url_part"].Value + "\\ "; //return value must have a trailing space or else the directive regex will not detect directives that contain only a url
            }            
        }

        static string OnTimeMatch(Match match)
        {
            var hour = uint.Parse (match.Groups["hour"].Value);
            var minute = uint.Parse (match.Groups["minute"].Value);

            if (hour <= 23 && minute <= 59)
            {
                return time_sigil + match.Value + "\\";
            }
            else
            {
                return match.Value;
            }
        }

        static string OnDirectiveMatch(Match match)
        {
            if (match.Value.ToUpper () == "[STRIKE]")
                return inline_directive_sigil + "STRIKE" + "]";

            var directive = match.Groups["directive"].Value;
            var data = match.Groups["data"].Value.Replace ("@", "&#064;");

            return inline_directive_sigil + directive + ":" + data + ']';
        }

        static string EmailMatch(string s)
        {
            var xs = new List<Tuple<string, string, int>> ();

            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '@')
                {
                    var local = s.Substring (0, i);
                    var domain = s.Substring (i + 1, s.Length - i - 1);

                    xs.Add (Tuple.Create (local, domain, i));
                }
            }

            var potentials = from x in xs
                             let p = new EmailParser.PotentialEmail (x.Item1, x.Item2, x.Item3)
                             where p.Test ()
                             select p;

            var actuals = new HashSet<EmailParser.PotentialEmail> ();

            foreach (var p in potentials)
            {
                if (p.Test ())
                {
                    foreach (var p_ in potentials)
                    {
                        if (p_ == p)
                            continue;
                        else
                            if (p.Overlaps (p_))
                                actuals.Add (p.Length >= p_.Length ? p : p_);
                            else
                                actuals.Add (p);
                    }
                }
            }

            int padding = 0;

            foreach (var actual in actuals)
            {
                int len = actual.ToString ().Length;

                var sigil = email_sigil + len.ToString ("000");

                s = s.Insert (padding + actual.LeftIndex, sigil);

                padding = padding + sigil.Length;
            }

            return s;
        }

        public TokenizedSpans(string textRun)
        {
            textRun = url_pattern.Replace (textRun, OnUrlMatch);
            textRun = time_pattern.Replace (textRun, OnTimeMatch);
            textRun = inline_directive_pattern.Replace (textRun, OnDirectiveMatch);
            textRun = strike_directive.Replace (textRun, OnDirectiveMatch);
            textRun = EmailMatch (textRun);

            textRun = empty_image.Replace (textRun, OnEmptyImageMatch);

            textRun = commercial_symbols.Replace (textRun, OnCommercialSymbol);
            
            if (textRun != null && textRun.Length > 0)
            {
                var chars = textRun.Select(c => new CharEx(c)).ToArray();

                var index = new Indexical<CharEx> (chars);

                var stack = new StackEx<SpanToken> (SpanToken.Start (ref index));

                while (++index)
                    stack.Push (stack.Top.Scan (ref index));
                
                tokens = stack.Reverse ();
            }
            else
            {
                tokens = Enumerable.Empty<SpanToken> ();
            }
        }

        public IEnumerator<SpanToken> GetEnumerator()
        {
            return tokens.GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }
    }
}