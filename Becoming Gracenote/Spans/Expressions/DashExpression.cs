using System;

namespace Gracenote
{
    sealed class DashExpression : TerminalSpan
    {
        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var neighbor = GetNeighbor ();

            if (neighbor is WordExpression && index.Value is WordToken)
            {
                parentStack.Pop ();
                
                var wordexp = parentStack.Pop () as WordExpression;

                var part1 = wordexp.word;
                var part2 = (index.Value as WordToken).GetString();

                return new HyphenatedWord (part1, part2);                
            }

            if (neighbor is HyphenatedWord && index.Value is WordToken)
            {
                parentStack.Pop ();

                var hyphenated = parentStack.Top as HyphenatedWord;

                hyphenated.AddPart (index.Value.GetString ());

                return hyphenated;
            }

            return base.Parse (ref index);
        }
        
        public override string GenerateCode()
        {
            return "-";    
        }
    }
}
