using System;
using System.Linq;
using System.Collections.Generic;

namespace Gracenote
{
    public abstract class SyntaxAttribute
    {

    }

    interface PlainText
    {
        string InnerText { get; }
    }
    
    public abstract class SpanSyntaxNode : Identity, StackEx<SpanSyntaxNode>.Membership
    {
        public abstract SpanSyntaxNode Parse(ref Indexical<SpanToken> index);

        protected StackEx<SpanSyntaxNode> parentStack;
        protected StackEx<SpanSyntaxNode>.Node node;

        readonly protected Guid id = Guid.NewGuid ();

        public Guid ID
        {
            get { return id; }
        }

        protected virtual void OnRegistration(StackEx<SpanSyntaxNode> stack) { }

        public virtual void Register(StackEx<SpanSyntaxNode> stack)
        {
            parentStack = stack;
            node = stack.TopNode;

            OnRegistration (stack);
        }

        public virtual void Deregister(StackEx<SpanSyntaxNode> stack)
        {
            return;
        }
    }

    public abstract class TerminalSpan : SpanSyntaxNode, IEnumerable<TerminalSpan>
    {
        protected readonly List<SyntaxAttribute> attributes = new List<SyntaxAttribute> ();
        
        public override SpanSyntaxNode Parse(ref Indexical<SpanToken> index)
        {
            var token = index.Value;

            if (token is EndOfline)
                return this;
            else
                return index.Value.GetStartNode (ref index);
        }
        
        public abstract string GenerateCode();

        public T GetAttribute<T>() where T : SyntaxAttribute
        {
            foreach (var attr in attributes)
            {
                var t = attr as T;

                if (t != null)
                    return t;
            }

            return null;
        }

        public virtual TerminalSpan GetImmediatelyNearestNonWhitesSpace()
        {
            if (node == null)
                throw new NodeIsNotOnTheStackException ();
            else
            {
                if (!(this is PartialWhitespaceExpression.WhitespaceExpression))
                    return this;
                else
                {
                    var cur = node.Next;

                    while (cur != null)
                        
                        if (cur.Value != null && cur.Value is TerminalSpan && !(cur.Value is PartialWhitespaceExpression.WhitespaceExpression))
                            return cur.Value as TerminalSpan;
                        
                        else
                            cur = cur.Next;
                }
            }

            return null;
        }

        public TerminalSpan GetNeighbor()
        {
            if (node == null)
                throw new NodeIsNotOnTheStackException ();
            else
            {
                if (node.Next != null)
                    return node.Next.Value as TerminalSpan;
                else
                    return null;
            }
        }

        class NodeIsNotOnTheStackException : InvalidOperationException
        {
            public NodeIsNotOnTheStackException()
                : base ("Failed because this node is not on the stack.")
            {

            }
        }

        public virtual IEnumerator<TerminalSpan> GetEnumerator()
        {
            #pragma warning disable
            
            if (false)
                yield return null;

            #pragma warning restore
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }        
    }

    public abstract class NonTerminalSpan : SpanSyntaxNode
    {
        protected void Pop()
        {
            parentStack.Pop ();
        }
    }

    public abstract class FromPartial<T> : TerminalSpan
        where T : NonTerminalSpan
    {
        protected readonly T partial;

        public FromPartial(T parital)
        {
            this.partial = parital;
        }
    }
}