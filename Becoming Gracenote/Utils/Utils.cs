namespace Gracenote
{
    using System.Text.RegularExpressions;
    using System.Text;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public static class Utils
    {
        public static readonly Regex ListPattern = new Regex (@"(?'leading'^\s*)((((?'digit'[0-9]+)|(?'lower'[a-z]+)|(?'upper'[A-Z]+))\.\s(?'trailing'\s?))|\*\s)"); //TODO: REVIEW: we're not using the groups.
        public static readonly Regex QuotePattern = new Regex (@"^\s*>");
        public static readonly Regex ReferencePattern = new Regex (@"^\[(?'refname'\S+)\]");
        public static readonly Regex URI_Pattern = new Regex (@"\w+://(?'cred'(\w+|-)(:(\w+|-))?@)?(?'host'(\w+|\.|-)+)(:\d+)?(?'path'(/|\w|#|-|(%([a-f][0-9]|[0-9][a-f]|\d\d|[a-f]{2})))*(?'query'\?(\w|=|&|#|-|\.|(%([a-f]|[0-9]){2}))*)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool LooksLikeURL(this string s)
        {
            return URI_Pattern.IsMatch (s.Trim ());
        }

        public static int GetLeadingWhiteSpaceCount(this string s)
        {
            var n = 0;

            for (var i = 0; i < s.Length; i++)
                if (!char.IsWhiteSpace (s[i]))
                    break;
                else
                    n++;

            return n;            
        }

        public static string TrimStart(this string s, int n)
        {
            var i = 0;

            while (i < Math.Min (n + 1, s.Length))
                if (char.IsWhiteSpace (s[i]))
                    i++;
                else
                    break;

            return s.Substring (i);
        }

        public static void AppendLine(this StringBuilder sb, string s, uint n)
        {
            if(s != ((char)26).ToString())
                sb.AppendLine ('\t'.Repeat (n) + s);
        }

        public static string Repeat(this char c, uint n)
        {
            var char_array = new char[n];

            for (var i = 0; i < n; i++)
                char_array[i] = c;

            return new string (char_array);
        }
    }

    //This is pretty much a vestigial type.
    public struct CharEx
    {
        readonly char c;

        public CharEx(char c)
        {
            this.c = c;
        }

        public bool IsWhiteSpace
        {
            get
            {
                return char.IsWhiteSpace (c);
            }
        }

        public bool IsDigit
        {
            get
            {
                return char.IsDigit (c);
            }
        }

        public bool IsLetter
        {
            get
            {
                return char.IsLetter (c);
            }
        }

        public bool IsLetterOrDigit
        {
            get
            {
                return char.IsLetterOrDigit (c);
            }
        }

        public bool IsGraveAccent
        {
            get
            {
                return c == '`';
            }
        }

        public bool IsDoubleQuote
        {
            get
            {
                return c == '"';
            }
        }

        public bool IsCaret
        {
            get
            {
                return c == '^';
            }
        }

        public bool IsLeftBracket
        {
            get
            {
                return c == '[';
            }
        }

        public bool IsRightBracket
        {
            get
            {
                return c == ']';
            }
        }

        public bool IsRightParen
        {
            get
            {
                return c == ')';
            }
        }

        public bool IsLeftParen
        {
            get
            {
                return c == '(';
            }
        }

        public static bool Equals(CharEx cx, char c)
        {
            return cx.c == c;
        }

        public static bool operator == (CharEx cx, char c)
        {
            return Equals (cx, c);
        }

        public static bool operator !=(CharEx cx, char c)
        {
            return !Equals (cx, c);
        }

        public static implicit operator CharEx(char c)
        {
            return new CharEx (c);
        }

        public static implicit operator char(CharEx cx)
        {
            return cx.c;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode ();
        }

        public override bool Equals(object obj)
        {
            return base.Equals (obj);
        }

        public override string ToString()
        {
            return c.ToString ();
        }
    }

    public struct Indexical<T> : IEnumerable<T>
    {
        public readonly IList<T> sequence;
        readonly int index;
        readonly uint count;

        public Indexical(IList<T> xs)
        {
            this.sequence = xs ?? Enumerable.Empty<T> ().ToList();
            count = (uint) Math.Abs (sequence.Count);

            index = xs.Any () ? 0 : -1;
        }

        private Indexical(IList<T> xs, int index, uint count)
        {
            sequence = xs;
            this.index = index;
            this.count = count;
        }

        public T Value
        {
            get
            {
                return sequence[index];
            }
        }

        public bool IsValid
        {
            get
            {
                return index < count && index >= 0;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public Indexical<T> Reset()
        {
            return new Indexical<T> (this.sequence);
        }

        public static bool operator true(Indexical<T> index)
        {
            return index.IsValid;
        }

        public static bool operator false(Indexical<T> index)
        {
            return !(index.IsValid);
        }

        public static Indexical<T> operator ++(Indexical<T> index)
        {
            return new Indexical<T> (index.sequence, index.index + 1, index.count);
        }

        public static Indexical<T> operator --(Indexical<T> index)
        {
            return new Indexical<T> (index.sequence, index.index - 1, index.count);
        }

        public static Indexical<T> operator +(Indexical<T> index, int n)
        {
            return new Indexical<T> (index.sequence, index.index + n, index.count);
        }

        public static Indexical<T> operator -(Indexical<T> index, int n)
        {
            return new Indexical<T> (index.sequence, index.index - n, index.count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new[] { Value }.Union (sequence.Skip (this.index)).GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }      
    }

    public sealed class MQListener : System.Diagnostics.TraceListener
    {
        static readonly System.Messaging.MessageQueue mq;

        static MQListener()
        {
            mq = new System.Messaging.MessageQueue (@".\Private$\trace");

            mq.Send ("inited");
        }

        public override void Write(string message)
        {
            mq.Send (message);
        }

        public override void WriteLine(string message)
        {
            mq.Send (message);
        }
    }
}