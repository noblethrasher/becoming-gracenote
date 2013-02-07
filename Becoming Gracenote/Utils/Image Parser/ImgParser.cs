using System;
using System.Collections.Generic;
using System.Linq;

namespace Gracenote
{
    public static class ImageDataParser
    {       
        public struct ParsedData
        {
            public readonly string Title, Path, ID;            
            
            public ParsedData(string title, string path, string id)
            {
                this.Title = title;
                this.Path = path;
                this.ID = id;
            }
        }

        public static ParsedData Parse(string data)
        {
            if (data != null && data.Length > 0)
            {
                var curr = Start (data[0]);
                var stk = new Stack<Token> ();

                stk.Push (curr);

                for (var i = 1; i < data.Length; i++)
                {
                    curr = curr.Parse (data[i]);

                    if (stk.Peek () != curr)
                        stk.Push (curr);
                }

                if (stk.Any (x => x is Error))
                    return new ParsedData ();
                else
                {
                    var title = stk.FirstOrDefault (x => x is Title);
                    var id = stk.FirstOrDefault (x => x is ID);
                    var path = stk.FirstOrDefault (x => x is Path);

                    return new ParsedData (title != null ? title.ToString () : null, path != null ? path.ToString () : null, id != null ? id.ToString () : null);
                }

            }
            else
                return new ParsedData ();            
        }

        static Token Start(char c)
        {
            if (char.IsLetterOrDigit (c))
                return new ID (c);

            if (c == '"')
                return new Title ();

            if (c == '/')
                return new Path (c);

            if(char.IsWhiteSpace(c))
                return new WhiteSpace();

            return new Error ();
        }
        
        abstract class Token
        {
            public abstract Token Parse(char c);
        }

        sealed class WhiteSpace : Token
        {
            public override Token Parse(char c)
            {
                if (char.IsWhiteSpace (c) || c == ']')
                    return this;

                if (c == '"')
                    return new Title ();

                if (Path.IsValidPathCharacter (c))
                    return new Path (c);

                return new Error ();
            }
        }

        sealed class ID : Token
        {
            List<char> chars = new List<char> ();

            public ID(char c)
            {
                chars.Add (c);
            }

            public override string ToString()
            {
                var id = new string (chars.ToArray ());

                int n;

                if (int.TryParse (id, out n))
                    id = "_" + id;

                return id;
            }

            public override Token Parse(char c)
            {
                if (char.IsWhiteSpace (c))
                    return new WhiteSpace ();

                if (c == '"')
                    return new Title ();

                if (c == ']')
                    return this;

                if (char.IsLetterOrDigit (c))
                {
                    chars.Add (c);
                    return this;
                }

                return new Error ();
            }
        }

        sealed class Error : Token
        {
            public override Token Parse(char c)
            {
                return this;
            }
        }

        sealed class Path : Token
        {
            List<char> chars = new List<char> ();

            const string path_symbols = "-._~:/?#@!$&'()*+,;="; //technically, '[' and ']' are legal (reserved) URL characters but we elide them here.
            
            public static bool IsValidPathCharacter(char c)
            {
                var _ = char.ToLower (c);
                
                if (char.IsDigit (_))
                    return true;

                var n = (int)_;

                if (n >= (int)'a' && n <= (int)'z')
                    return true;

                return path_symbols.Contains (_);
            }

            public Path(char c)
            {
                chars.Add (c);
            }

            public override Token Parse(char c)
            {
                if (IsValidPathCharacter (c))
                {
                    chars.Add (c);
                    return this;
                }

                if (c == '"')
                    return new Title ();

                if (c == ']')
                    return this;

                if (char.IsWhiteSpace (c))
                    return new WhiteSpace ();

                return new Error ();
            }

            public override string ToString()
            {
                return new string (chars.ToArray ());
            }
        }

        sealed class Title : Token
        {
            List<char> chars = new List<char> ();

            public override Token Parse(char c)
            {
                if (c == '"')
                    return new WhiteSpace ();

                if (c == ']' || c == '[')
                    return new Error ();

                chars.Add (c);
                return this;
            }

            public override string ToString()
            {
                return new string (chars.ToArray ());
            }
        }
    }
}


