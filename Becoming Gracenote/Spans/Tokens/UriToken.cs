using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Gracenote
{
    public sealed class UriToken : SpanToken
    {
        public UriToken()            
        {
            
        }

        bool done = false;

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            if (done)
            {                
                if(index.Value.IsWhiteSpace && (index +1).IsValid)
                    index++; //HACK: eat the trailing space.

                return Start (ref index);
            }
            
            if (index.Value != '\\')
            {
                chars.Add (index.Value);                
                return this;
            }
            else
            {
                done = true;
                return this;
            }
        }       

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {

            List<char> uri_xs = new List<char>(), title_xs = new List<char>();

            var k = new[] { uri_xs, title_xs };
            int ndx = 0;

            for (var i = 0; i < chars.Count; i++)
            {
                if (chars[i] == '>')
                {
                    ndx = 1;
                    continue;
                }

                k[ndx].Add (chars[i]);
            }

            var url = new string (uri_xs.ToArray ());

            var title = title_xs.Count > 0 ? new string (title_xs.ToArray ()) : url;


            return new HyperLinkExpression (url, url, title);
        }
    }

    public sealed class NakedUrlToken : SpanToken
    {
        public NakedUrlToken()            
        {
            
        }

        bool done = false;

        public override SpanToken Scan(ref Indexical<CharEx> index)
        {
            if (done)
                return Start (ref index);

            if (index.Value != '\\')
            {
                chars.Add (index.Value);
                return this;
            }
            else
            {
                done = true;
                return this;
            }
        }

        public override SpanSyntaxNode GetStartNode(ref Indexical<SpanToken> index)
        {
            int i = 0;
            

            while (i < chars.Count && chars[i] != '>')
                i++;
            
            var full_url = new string (chars.Take(i).ToArray());
            var display = full_url.Split (new[] { "://" }, System.StringSplitOptions.None)[1];

            return new HyperLinkExpression (full_url, display, full_url);
        }
    }
}