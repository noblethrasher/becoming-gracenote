using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Gracenote
{
    public abstract class Container<StartToken, EndToken> : NonTerminalSpan
        where StartToken : SpanToken
        where EndToken : SpanToken
    {
        protected readonly ItemList items = new ItemList ();
        protected readonly Indexical<SpanToken> index;
        
        public Container(Indexical<SpanToken> index, Container<StartToken, EndToken> parent = null)
        {
            this.index = index;            
        }

        public class ItemList : IEnumerable<Item>
        {
            List<Item> items = new List<Item> ();

            public void Add(Item item)
            {
                item.parent = this;
                items.Add (item);
            }

            public IEnumerator<Item> GetEnumerator()
            {
                return items.GetEnumerator ();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator ();
            }
        }

        protected abstract IEnumerable<SpanToken> GetStartSymbol();

        int balance = typeof (StartToken) == typeof (EndToken) ? 0 : 1;

        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var token = index.Value;

            if (token is EndOfline) //if true, then we've reached the end of the line without closing this container.
            {
                var xs = new List<TextFragment.Item> ();

                var start_symbol = GetStartSymbol ();

                if (start_symbol != null && start_symbol.Any())
                    xs.Add (new Item.SubItem (start_symbol));

                foreach (var item in items)
                    xs.Add (item);

                return new TextFragment (xs);
            }

            if (token is StartToken)
                balance++;

            if (token is EndToken)
                balance--;

            if (token is EndToken && balance == 0)
                return Complete (ref index);
            
            Add (token);
                return this;
        }

        public abstract class Item
        {
            public ItemList parent;

            public abstract void Append(SpanToken token);
            public abstract string GenerateCode();
            public abstract IEnumerable<TerminalSpan> GetTerminals();

            public static implicit operator TextFragment.Item(Item item)
            {
                return item.ToTextFragmentItem ();
            }

            public abstract TextFragment.Item ToTextFragmentItem();

            internal class SubContainer : Item
            {
                protected readonly TerminalSpan subcontainer;

                public SubContainer(TerminalSpan subcontainer)
                {
                    this.subcontainer = subcontainer;
                }

                public override IEnumerable<TerminalSpan> GetTerminals()
                {
                    return new[] { subcontainer };
                }

                public override void Append(SpanToken token)
                {
                    parent.Add (new SubItem (token));
                }

                public override string GenerateCode()
                {
                    return subcontainer.GenerateCode ();
                }

                public override TextFragment.Item ToTextFragmentItem()
                {
                    return new TextFragment.Item.SubTerminal (subcontainer);
                }
            }

            public class SubItem : Item
            {
                List<SpanToken> tokens = new List<SpanToken> ();

                public SubItem(SpanToken token)
                {
                    tokens.Add (token);
                }

                public SubItem(IEnumerable<SpanToken> tokens)
                {
                    this.tokens.AddRange (tokens);
                }

                public override IEnumerable<TerminalSpan> GetTerminals()
                {
                    return new ParsedTextRun(tokens);
                }

                public override void Append(SpanToken token)
                {
                    tokens.Add (token);
                }

                public override string GenerateCode()
                {
                    return new ParsedTextRun (tokens).GenerateCode ();
                }

                public override TextFragment.Item ToTextFragmentItem()
                {
                    return new TextFragment.Item.SubText (tokens);
                }
            }
        }

        protected void Add(TerminalSpan node)
        {
            items.Add (new Item.SubContainer (node));
        }

        protected void Add(SpanToken token)
        {
            var last = items.LastOrDefault ();

            if (last == null)
                items.Add (new Item.SubItem(token));
            else
                last.Append (token);            
        }

        protected abstract TerminalSpan GetCompletion();

        protected static string GenerateSubCode(IEnumerable<Item> items)
        {
            var sb = new StringBuilder ();

            foreach (var item in items)
                sb.Append (item.GenerateCode ());

            return sb.ToString ();
        }

        protected static string GenerateSubCode(Container<StartToken, EndToken> container)
        {
            return GenerateSubCode (container.items);
        }

        protected static IEnumerable<SpanToken> AsLiterals(params char[] chars)
        {
            return chars.Select (c => new LiteralToken (c));
        }

        protected virtual SpanSyntaxNode Complete(ref Indexical<SpanToken> index)
        {
            var exp = parentStack.Pop ();

            return GetCompletion ();            
        }
    }    
}