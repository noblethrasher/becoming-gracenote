using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Gracenote
{
    public sealed class Parsed : IEnumerable<TerminalLine>
    {
        public static Action<string> WordCountAccumulator;
        
        IEnumerable<TerminalLine> terminals;

        static string[] separators = new [] { "\r\n", "\r", "\n" };

        [ThreadStatic]
        static string relaive_media_path;

        public static string RelativeMediaPath
        {
            get
            {
                return relaive_media_path;
            }
            set
            {
                relaive_media_path = value;
            }
        }
        
        public Parsed(string text) : this(text != null ? text.Split(separators, StringSplitOptions.None) : Enumerable.Empty<string>())
        {
            
        }

        public Parsed(IEnumerable<LineToken> tokens)
        {
            ReferenceExpression.order = 0;
            
            if (tokens.Any ())
            {
                var tokenized = tokens.ToList ();

                tokenized.Add (new EndOfFileToken ());

                var i = 0;

                var stack = new StackEx<SyntaxNode> (tokenized[0].StartNode (ref i));

                while (++i < tokenized.Count)
                    stack.Push (stack.Top.Parse (tokenized[i], ref i));


                terminals = (from n in stack
                             let term = n as TerminalLine
                             where term != null
                             select term).Reverse ();
            }
            else
            {
                terminals = Enumerable.Empty<TerminalLine> ();
            }
        }

        public Parsed(IEnumerable<string> lines)
        {
            if (lines.Any())
            {
                var tokenized = new Tokenized (lines).ToList ();

                tokenized.Add (new EndOfFileToken ());

                var i = 0;

                var stack = new StackEx<SyntaxNode> (tokenized[0].StartNode (ref i));

                while (++i < tokenized.Count)
                    stack.Push (stack.Top.Parse (tokenized[i], ref i));

                terminals = (from n in stack
                             let term = n as TerminalLine
                             where term != null
                             select term).Reverse ();

                
            }
            else
                terminals = Enumerable.Empty<TerminalLine> ();
        }

        public IEnumerator<TerminalLine> GetEnumerator()
        {
            return terminals.GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }

        public string GenerateCode()
        {
            var sb = new StringBuilder ();

            foreach (var exp in this.terminals)
                sb.AppendLine (exp.GenerateCode ());

            return sb.ToString ();
        }
    }
}