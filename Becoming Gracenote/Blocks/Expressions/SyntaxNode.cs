namespace Gracenote
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    public abstract class SyntaxNode : Identity, StackEx<SyntaxNode>.Membership
    {
        protected readonly List<BlockExpressionAttribute> attributes = new List<BlockExpressionAttribute> ();
        
        public abstract SyntaxNode Parse(LineToken token, ref int i);

        readonly Guid id = Guid.NewGuid ();
        
        public Guid ID
        {
            get { return id; }
        }

        protected StackEx<SyntaxNode> parentStack;

        public void Register(StackEx<SyntaxNode> stack)
        {
            this.parentStack = stack;

            OnRegistration (stack);
        }

        protected virtual void OnRegistration(StackEx<SyntaxNode> stack)
        {

        }

        public void Deregister(StackEx<SyntaxNode> stack)
        {
            return;
        }

        public T GetAttribute<T>()
            where T : BlockExpressionAttribute
        {
            return attributes.Select (x => x as T).FirstOrDefault ();
        }
    }

    public abstract class TerminalLine : SyntaxNode
    {
        public override SyntaxNode Parse(LineToken token, ref int i)
        {
            if (token is EndOfFileToken)
                return this;
            else            
                return token.StartNode (ref i);
        }

        public abstract string GenerateCode(uint n = 0);
    }

    public sealed class SyntaxError : TerminalLine
    {
        readonly bool recover;
        readonly string message;
        readonly int line_number;

        public SyntaxError(int line_number, string message = null, bool recover = false)
        {
            this.line_number = line_number;
            this.recover = recover;
            this.message = message ?? (recover ? "" : "Unrecoverable ") + "Syntax Error";
        }

        public override SyntaxNode Parse(LineToken token, ref int i)
        {
            if (!recover)
                return this;
            else
                return base.Parse (token, ref i);
        }

        public override string GenerateCode(uint n = 0)
        {
            return message;
        }
    }

    public abstract class NonTerminalLine : SyntaxNode
    {

    }
}