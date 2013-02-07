using System.Linq;
using System.Collections.Generic;

namespace Gracenote
{
    public sealed class WordExpression : TerminalSpan, PlainText
    {
        public readonly string word;

        public WordExpression(string word)        
        {            
            this.word = word;

            bool handlesAcronym = true;

            for (var i = 0; i < word.Length; i++)
                if (char.IsLower (word[i]))
                    handlesAcronym = false;
            
            if(handlesAcronym)
                attributes.Add(new _HandlesAcronym(this));

            if (Parsed.WordCountAccumulator != null)
                Parsed.WordCountAccumulator (word);
        }

        public override string GenerateCode()
        {
            return word;
        }

        sealed class _HandlesAcronym : HandlesAcronym
        {
            readonly WordExpression word;

            public _HandlesAcronym(WordExpression word)
            {
                this.word = word;
            }
            
            public override TerminalSpan Handle(string text)
            {
                word.parentStack.Pop();
                
                return new Acronym(word.word, text);
            }
        }

        public string InnerText
        {
            get { return word; }
        }
    }

    public sealed class HyphenatedWord : TerminalSpan, PlainText
    {
        List<string> parts = new List<string> ();

        public HyphenatedWord(string part1, string part2)
        {
            parts.Add (part1);
            parts.Add (part2);
        }

        public override string GenerateCode()
        {
            return string.Join ("-", parts);
        }

        public string InnerText
        {
            get { return GenerateCode (); }
        }

        public void AddPart(string word)
        {
            parts.Add (word);
        }
    }
}