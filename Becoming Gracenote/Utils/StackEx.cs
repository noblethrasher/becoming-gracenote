namespace Gracenote
{
    using System;
    using System.Collections.Generic;

    public interface Identity
    {
        Guid ID {get; }
    }

    public sealed class StackEx<T> : IEnumerable<T>
        where T : Identity, StackEx<T>.Membership
    {
        
        public interface Membership
        {
            void Register(StackEx<T> stack);
            void Deregister(StackEx<T> stack);
        }

        public int Count { get; private set; }

        public class Node
        {
            public  T Value;
            public Node Next;

            public Node(T value, Node next)
            {
                Value = value;
                Next = next;
            }

            public static implicit operator T(Node node)
            {
                return node.Value;
            }
        }

        Node top;
        
        public StackEx(T value)
        {
            Push (value);
        }

        public StackEx()
        {

        }

        public T Top
        {
            get
            {
                return top;
            }
        }

        public Node TopNode
        {
            get
            {
                return top;
            }
        }

        public StackEx<T> Push(T value)
        {
            if (top == null || value.ID != top.Value.ID)
            {
                top = new Node (value, top);
                Count++;
                value.Register (this);
            }

            return this;
        }

        public StackEx<T> PushUnconditionally(T value)
        {
            top = new Node (value, top);

            value.Register (this);

            Count++;

            return this;
        }

        public T Pop()
        {
            var temp = top;
            top = top.Next;
            Count--;

            temp.Value.Deregister (this);

            

            return temp;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = top;

            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }
    }
}