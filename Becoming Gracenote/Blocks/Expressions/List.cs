using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Text.RegularExpressions;

namespace Gracenote
{
    public sealed class PartialList : NonTerminalLine
    {
        readonly ListToken initial;
        readonly PartialList parent;

        List<Item> items = new List<Item> ();

        public PartialList(ListToken list, PartialList parent = null)
        {
            this.initial = list;
            this.parent = parent;

            items.Add (new Item (list));
        }

        public override SyntaxNode Parse(LineToken token, ref int i)
        {
            var blanks = token.NumberOfBlankLines;

            if (blanks > 0)
            {
                if (blanks > 1)            
                    return Complete (token, ref i);

                if (blanks == 1)
                {
                    items.Last ().Append ("");
                    return this;
                }
            }
            
            var diff = initial.LeadingWhiteSpaceCount - token.LeadingWhiteSpaceCount; //we want the indentation level reletive to this line

            if (diff == 0)
            {
                var t = token as ListToken;

                if (t != null)
                {
                    items.Add (new Item (t));
                    return this;
                }
                else
                    return Complete (token, ref i);
            }
            else
            {
                if (diff > 0) //the indentation level is less than this line so we definitely cannot add it to this list.
                    return Complete (token, ref i);

                else //diff < 0
                {
                    var trimmed = Utils.TrimStart (token, initial.SpacesToTrim);

                    items.Last ().Append (trimmed);
                    return this;
                }
            }            
        }

        SyntaxNode Complete(LineToken token, ref int i)
        {
            parentStack.Pop ();

            var completed = new CompletedList (this);

            if (this.parent != null)
            {
                parent.items.Last().Append (completed);
                return parent;
            }
            else
            {
                parentStack.Push (completed);
                return token.StartNode (ref i);
            }
        }

        sealed class Item
        {
            public ListItemCollection container;

            List<SubItem> subItems = new List<SubItem> ();
            
            public Item(ListToken token)
            {
                subItems.Add (token.GetProperString ());
            }

            public void Add(SubItem item)
            {
                subItems.Add (item);
                item.container = this;
            }

            public void Append(string line)
            {
                var last = subItems.Last ();

                last.Append (line);
            }

            public void Append(CompletedList list)
            {
                subItems.Last ().Append (list);
            }

            public string GenerateCode(uint n = 0)
            {
                var sb = new StringBuilder ();

                foreach (var sub in subItems)
                {
                    sb.AppendLine (sub.GenerateCode (n));
                }

                return sb.ToString ();
            }
        }

        abstract class SubItem
        {
            public Item container;

            public abstract void Append(string line);
            public abstract void Append(CompletedList list);
            public abstract string GenerateCode(uint n);

            public static implicit operator SubItem(string line)
            {
                return new SubText (line);
            }

            public static implicit operator SubItem(CompletedList list)
            {
                return new SubList (list);
            }

            sealed class SubList : SubItem
            {
                readonly CompletedList list;
                
                public SubList(CompletedList list)
                {
                    this.list = list;
                }
                
                public override void Append(string line)
                {
                    this.container.Add (new SubText(line));
                }

                public override void Append(CompletedList list)
                {
                    this.container.Add (new SubList (list));
                }

                public override string GenerateCode(uint n)
                {
                    return list.GenerateCode (n);
                }
            }

            sealed class SubText : SubItem
            {
                List<string> lines = new List<string> ();
                
                public SubText(string line)
                {
                    lines.Add (line);
                }

                public override void Append(CompletedList list)
                {
                    this.container.Add (new SubList (list));                    
                }

                public override void Append(string line)
                {
                    lines.Add (line);
                }

                public override string GenerateCode(uint n)
                {
                    var parsed = new Parsed (lines);

                    var sb = new StringBuilder ();

                    foreach (var exp in parsed)
                        sb.AppendLine (exp.GenerateCode (n));

                    return sb.ToString ();
                }
            }
        }

        sealed class ListItemCollection : IEnumerable<Item>
        {
            Stack<Item> items = new Stack<Item> ();
            
            public void Add(Item item)
            {
                item.container = this;
            }

            public void Append(string line)
            {
                items.Peek ().Append (line);
            }

            public void Append(CompletedList list)
            {
                items.Peek ().Append (list);
            }

            public IEnumerator<Item> GetEnumerator()
            {
                return new Stack<Item> (items).GetEnumerator ();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator ();
            }
        }

        sealed class CompletedList : TerminalLine
        {
            readonly PartialList p;
            
            public CompletedList(PartialList p)
            {
                this.p = p;
            }

            public override string GenerateCode(uint n = 0)
            {
                var sb = new StringBuilder ();

                var tag = p.initial.list_style is ListStyleType.Unordered ? "ul" : "ol";

                sb.AppendLine ("<" + tag + (tag[0] == 'o' ? " style=\"list-style-type:" + p.initial.list_style.GetStyle() + "\"" : "")  + ">", n); //we don't need to list the render an explicit style attribute for unordered list

                foreach (var item in p.items)
                {
                    sb.AppendLine ("<li>", n + 1);

                    var s = item.GenerateCode (n + 2);
                    
                    sb.AppendLine (s.TrimEnd()); //HACK: Why are extra newlines being added?

                    sb.AppendLine ("</li>", n + 1);
                }

                sb.AppendLine ("</" + tag + ">", n);

                return sb.ToString ();
            }
        }
    }
}