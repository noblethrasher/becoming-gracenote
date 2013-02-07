using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gracenote.EmailParser
{
    enum State
    {
        Feeding,
        NotFeeding,
        Error,
        Done
    }

    enum EmailPart
    {
        Local = 0,
        Domain = 1
    }

    sealed class PotentialEmail
    {
        readonly string local_part, domain_part;
        public readonly int start;

        Tokenized domain = null, local = null;

        public int LeftIndex { get; private set; }
        public int RightIndex { get; private set; }

        public PotentialEmail(string local, string domain, int start)
        {
            local_part = local;
            domain_part = domain;

            this.start = start;

            this.local = new Tokenized (local_part, EmailPart.Local);
            this.domain = new Tokenized (domain_part, EmailPart.Domain);
            
            //If we're inside of a parentheis then truncate
            
            if (this.domain_part.EndsWith (")"))
                this.domain_part = new string (domain_part.Take (domain_part.Length - 1).ToArray ());

            if (this.local_part.StartsWith("("))
                this.local_part = new string (local_part.Skip(1).ToArray ());

            LeftIndex = start - this.local.Sum (x => x.GetString ().Length);
            RightIndex = this.start + this.domain.Sum (x => x.GetString ().Length);
        }

        public bool Test() //
        {
            return local.Success && domain.Success;
        }

        public bool Overlaps(PotentialEmail other) //If there is an overlap with another potential email string then we want to return the one that is the most encompassing.
        {
            var this_start = LeftIndex;
            var this_end = RightIndex;

            var other_start = other.start - other.local.Sum (x => x.GetString ().Length);
            var other_end = other.start + other.domain.Sum (x => x.GetString ().Length);

            return (this_start <= other_start && this_end >= other_start) || (other_start <= this_start && other_end >= this_end);
        }

        public int Length
        {
            get { return local_part.Length + domain_part.Length + 1; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder ();

            foreach (var token in local)
                sb.Append (token.GetString ());

            sb.Append ("@");

            foreach (var token in domain.Reverse ())
                sb.Append (token.GetString ());

            var ret = sb.ToString ();
            
            return ret;
        }
    }

    abstract class Token
    {
        protected static readonly char[] regular;
        protected static readonly char[] special = "(),:;<>@[]".ToCharArray ();

        public bool IsTerminal { get; protected set; }

        static Token()
        {
            regular = (from n in Enumerable.Range (48, 10) select (char)n) //digits
                        .Union (from n in Enumerable.Range (65, 26) select (char)n) //upper case ascii letters
                        .Union (from n in Enumerable.Range (97, 26) select (char)n) //lower case ascii letters
                        .Union ("!#$%&'*+-/=?^_`{|}~") //allowed symbols
                        .ToArray ();
        }

        readonly protected Stack<char> chars = new Stack<char> ();

        public State Disposition { get; protected set; }

        public abstract Token Lex(ref Indexical<char> index);

        protected void Add(char c)
        {
            chars.Push (c);
        }

        public static bool IsRegularLocalCharacter(char c)
        {
            for (var i = 0; i < regular.Length; i++)
                if (regular[i] == c)
                    return true;

            return false;
        }

        public static bool IsAllowedLabelCharacter(char c)
        {
            int n = char.ToLower (c);

            return (n >= 48 && n <= 57) || (n >= 97 && n <= 122) || c == '_';
        }

        public abstract string GetString();

        public virtual int Length
        {
            get
            {
                return chars.Count;
            }
        }
    }

    sealed class Error : URL
    {
        public override string GetString()
        {
            return "";
        }

        public Error()
        {
            Disposition = State.Error;
        }

        public override Token Lex(ref Indexical<char> index)
        {
            return this;
        }
    }

    sealed class QuotedString : Token
    {
        public override string GetString()
        {
            return '"' + new string (chars.ToArray ()) + '"';
        }

        public override Token Lex(ref Indexical<char> index)
        {
            if (Disposition == State.Feeding)
            {
                var c = index.Value;

                if (c == '"')
                {
                    var next = index + 1;

                    if (next.IsValid && next.Value == '\\')
                    {
                        index = next;
                        Add (c);
                        Add (next.Value);

                        return this;
                    }
                    else
                    {
                        Disposition = State.NotFeeding;
                        return this;
                    }
                }

                if (c == '\\')
                {
                    var next = index + 1;

                    if (next.IsValid && (next.Value == '\\' || next.Value == '"'))
                    {
                        index = next;
                        Add (c);
                        Add (next.Value);
                    }
                    else
                        Disposition = State.Error;

                    return this;
                }
                else
                {
                    Add (c);
                    return this;
                }
            }
            if (Disposition == State.NotFeeding)
            {
                var c = index.Value;

                if (c == '.')
                    return new Dot (EmailPart.Local);

                if (c == ')')
                    return new Comment (EmailPart.Local);

                if (char.IsWhiteSpace (c) || c == '(')
                {
                    Disposition = State.Done;
                    return this;
                }
            }

            Disposition = State.Error;
            return this;
        }

        public override int Length
        {
            get
            {
                return base.Length + 2;
            }
        }
    }

    sealed class Word : Token
    {
        public Word(char c)
        {
            Add (c);
        }

        public override string GetString()
        {
            return new string (chars.ToArray ());
        }

        public override Token Lex(ref Indexical<char> index)
        {
            var c = index.Value;

            if (char.IsWhiteSpace (c) | c == '(')
            {
                Disposition = State.Done;
                return this;
            }

            if (IsRegularLocalCharacter (c))
            {
                this.Add (c);
                return this;
            }

            if (c == '.')
            {
                Disposition = State.NotFeeding;
                return new Dot (EmailPart.Local);
            }

            if (c == ')')
            {
                Disposition = State.NotFeeding;
                return new Comment (EmailPart.Local);
            }

            Disposition = State.Error;
            return this;
        }
    }

    sealed class Dot : Token
    {
        readonly EmailPart part;

        public Dot(EmailPart part)
        {
            this.part = part;
        }

        public override string GetString()
        {
            return ".";
        }

        public override Token Lex(ref Indexical<char> index)
        {
            var c = index.Value;

            if (part == EmailPart.Local)
            {
                if (IsRegularLocalCharacter (c))
                    return new Word (c);

                if (c == '"')
                    return new QuotedString ();

                Disposition = State.Error;
                return this;
            }
            else
            {
                if (IsAllowedLabelCharacter (c))
                    return new URL.Label (c);
                if (char.IsWhiteSpace (c))
                {
                    IsTerminal = true;
                    Disposition = State.Done;
                    return this;
                }

                return new Error ();
            }
        }

        public override int Length
        {
            get
            {
                return 1;
            }
        }
    }

    sealed class Comment : Token
    {
        readonly bool at_start;

        int balance = -1;

        readonly EmailPart part;

        char[] delimeters = new[] { ')', '(' };

        public Comment(EmailPart part, bool at_start = false)
        {
            this.at_start = at_start;

            this.part = part;
        }

        public override Token Lex(ref Indexical<char> index)
        {
            var c = index.Value;

            if (c == ')')
                if (part == EmailPart.Local)
                    balance--;
                else
                    balance++;

            if (c == '(')
                if (part == EmailPart.Local)
                    balance++;
                else
                    balance--;

            if (Disposition == State.Feeding)
            {
                var delimiter = delimeters[(((int)part) + 1) % 2];

                if (c != delimiter)
                {
                    Add (c);
                    return this;
                }
                if (c == delimiter)
                {
                    if (balance == 0)
                    {
                        IsTerminal = true;
                        Disposition = State.NotFeeding;
                        return this;
                    }
                    else
                    {
                        Add (c);
                        return this;
                    }
                }

                Disposition = State.Error;
                return this;
            }
            else
            {                
                var trigger = part == EmailPart.Domain ? ')' : '(';

                if (char.IsWhiteSpace (c) || c == trigger)
                {
                    if (at_start)
                        Disposition = State.Error;
                    else
                        Disposition = State.Done;

                    return this;
                }

                if (part == EmailPart.Local)
                {
                    if (IsRegularLocalCharacter (c))
                        return new Word (c);

                    if (c == '"')
                        return new QuotedString ();
                }
                else
                {
                    if (IsAllowedLabelCharacter (c))
                        return new URL.Label (c);

                    if (c == '[')
                        return new IpAddress ();
                }
            }

            Disposition = State.Error;
            return this;
        }

        public override int Length
        {
            get
            {
                return base.Length + 2;
            }
        }

        public override string GetString()
        {
            var chrs = chars.ToArray ();

            if (part == EmailPart.Domain)
                Array.Reverse (chrs);
            
            return "(" + new string (chrs) + ")";
        }
    }

    class Tokenized : IEnumerable<Token>
    {
        public bool Success
        {
            get
            {
                return tokens.Any () && tokens.First ().Disposition == State.Done && tokens.All (x => x.Disposition != State.Error);
            }
        }

        readonly IEnumerable<Token> tokens;

        static readonly Func<char, Token>[] f = new Func<char, Token>[] { StartLocalPart, StartDomainPart };

        public Tokenized(string s, EmailPart part)
        {
            if (s != null && s.Length > 0)
            {
                if (part == EmailPart.Local)
                    s = new string (s.Reverse ().ToArray ());

                var token = f[(int)part] (s[0]);
                var stack = new Stack<Token> ();

                stack.Push (token);

                s = s + " ";

                var index = new Indexical<char> (s.ToCharArray ());

                while (++index)
                {
                    var curr = stack.Peek ();

                    if (curr.Disposition != State.Error && curr.Disposition != State.Done)
                    {
                        curr = curr.Lex (ref index);

                        if (curr != stack.Peek ())
                            stack.Push (curr);
                    }
                    else
                        break;
                }

                tokens = stack;
            }
            else
            {
                tokens = Enumerable.Empty<Token> ();
            }
        }

        static Token StartLocalPart(char c)
        {
            if (Token.IsRegularLocalCharacter (c))
                return new Word (c);

            if (c == '"')
                return new QuotedString ();

            if (c == ')')
                return new Comment (EmailPart.Local, at_start: true);

            return new Error ();
        }

        static Token StartDomainPart(char c)
        {
            if (URL.IsRegularLocalCharacter (c))
                return new URL.Label (c);

            if (c == '[')
                return new IpAddress ();

            if (c == '(')
                return new Comment (EmailPart.Domain, true);

            return new Error ();
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return tokens.GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }
    }

    sealed class IpAddress : Token
    {
        static readonly char[] allowed_chars = "0123456789abcdef.".ToCharArray ();

        bool is_ipv6 = false;

        public override string GetString()
        {
            return '[' + new string (chars.Reverse ().ToArray ()) + ']';
        }

        public override Token Lex(ref Indexical<char> index)
        {
            var c = char.ToLower (index.Value);

            if (this.Disposition == State.Feeding)
            {
                if (c == 'i' && index.Index == 1)
                {
                    if (index + 5)
                    {
                        char[] cs = new char[4];

                        for (var i = 1; i <= 4; i++)
                            cs[i - 1] = char.ToLower ((index + i).Value);

                        if (cs[0] == 'p' && cs[1] == 'v' && cs[2] == '6' && cs[3] == ':')
                        {
                            Add (c);

                            for (var i = 1; i <= 4; i++)
                                Add (cs[i - 1]);

                            index = index + 4;
                            is_ipv6 = true;
                            return this;
                        }
                    }
                }

                if (c == ':' && is_ipv6)
                {
                    Add (c);
                    return this;
                }

                if (c == ']')
                {
                    this.Disposition = State.Done;
                    return this;
                }

                if (c == '.')
                {
                    if (chars.Any () && chars.Peek () == '.')
                    {
                        this.Disposition = State.Error;
                        return this;
                    }
                    else
                    {
                        Add (c);
                        return this;
                    }
                }

                for (var i = 0; i < allowed_chars.Length; i++)
                {
                    if (allowed_chars[i] == c)
                    {
                        Add (c);
                        return this;
                    }
                }

                Disposition = State.Error;
                return this;
            }
            else
                Disposition = State.Error;

            return this;
        }
    }

    abstract class URL : Token
    {
        public static Token Create(char c)
        {
            if (c == '.')
                return new Dot (EmailPart.Domain);

            if (c == '(')
                return new Comment (EmailPart.Domain);

            if (IsAllowedLabelCharacter (c))
                return new Label (c);

            return new Error ();
        }

        public sealed class Label : URL
        {
            public override string GetString()
            {
                return new string (chars.Reverse ().ToArray ());
            }

            public Label(char c)
            {
                chars.Push (c);
            }

            public override Token Lex(ref Indexical<char> index)
            {
                var c = index.Value;

                if (Disposition == State.Feeding)
                {
                    if (c == '.')
                    {
                        Disposition = State.NotFeeding;
                        return new Dot (EmailPart.Domain);
                    }

                    if (char.IsWhiteSpace (c) || c == ')')
                    {
                        Disposition = State.Done;
                        return this;
                    }

                    if (c == '(')
                        return new Comment (EmailPart.Domain);

                    if (IsAllowedLabelCharacter (c))
                        chars.Push (c);
                    else
                        Disposition = State.Error;

                    return this;
                }

                Disposition = State.Error;
                return this;
            }
        }
    }
}