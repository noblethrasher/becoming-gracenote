using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System;

namespace Gracenote
{
    public abstract class HeaderExpression : TerminalLine
    {
        static readonly MD5 md5 = MD5.Create ();

        readonly string line, content;
        readonly int level;

        public new string ID { get; private set; }
        
        protected HeaderExpression(string line, int level)
        {
            this.line = line;
            this.level = level;

            var parsed = new ParsedTextRun (line);

            content = parsed.GenerateCode ();

            ID = "h" + level.ToString () + "_" + Convert.ToBase64String (md5.ComputeHash (Encoding.Unicode.GetBytes (content))).Replace ('+', '-').Replace ('/', '_').Replace ('=', '_');
        }

        public override string GenerateCode(uint n = 0)
        {
            return string.Format ("<h{0} id=\"{2}\">{1}</h{0}>", level, content, ID);
        }

        public sealed class H1 : HeaderExpression
        {
            public H1(string line) : base (line, 1) { }
        }

        public sealed class H2 : HeaderExpression
        {
            public H2(string line) : base (line, 2) { }
        }

        public sealed class H3 : HeaderExpression
        {
            public H3(string line) : base (line, 3) { }
        }

        public sealed class H4 : HeaderExpression
        {
            public H4(string line) : base (line, 4) { }
        }

        public sealed class H5 : HeaderExpression
        {
            public H5(string line) : base (line, 5) { }
        }

        public sealed class H6 : HeaderExpression
        {
            public H6(string line) : base (line, 6) { }
        }
    }
}