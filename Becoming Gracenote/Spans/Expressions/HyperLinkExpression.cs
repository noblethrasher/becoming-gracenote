using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace Gracenote
{
    public sealed class HyperLinkExpression : TerminalSpan
    {
        readonly static bool no_follow;
        readonly static bool open_new_window;

        static HyperLinkExpression()
        {
            var nofollow = ConfigurationManager.AppSettings["noFollow"];
            var open_new = ConfigurationManager.AppSettings["openNew"];

            if (nofollow != null && (nofollow.Equals ("yes", System.StringComparison.OrdinalIgnoreCase) || nofollow.Equals ("true", System.StringComparison.OrdinalIgnoreCase)))
                no_follow = true;

            if (open_new != null && (open_new.Equals ("yes", System.StringComparison.OrdinalIgnoreCase) || open_new.Equals ("true", System.StringComparison.OrdinalIgnoreCase)))
                open_new_window = true;                
        }

        readonly string title, text;
        
        public bool IgnoreOpenInNewWindow
        {
            get;
            set;
        }

        string _href;
        public string HREF
        {
            get
            {
                return _href;
            }
        }

        public HyperLinkExpression(string href, string text, string title)
        {
            this._href = href;
            this.title = title;
            this.text = text;
        }

       
        //public HyperLinkExpression(HyperLinkReference href, string text = null, string title = null)
        //    : this (href.ToString (), text, title)
        //{

        //}

        //public HyperLinkExpression(string href, string text = null, string title = null)
        //{
        //    this.href = href;
        //    this.text = text;
        //    this.title = title;
        //}

        protected override void OnRegistration(StackEx<SpanSyntaxNode> stack)
        {
            if (node != null)
            {
                var neighbor = GetNeighbor ();

                if (neighbor != null)
                {
                    var non_whitespace = neighbor.GetImmediatelyNearestNonWhitesSpace ();

                    if (non_whitespace != null)
                    {
                        var accept_href = non_whitespace.GetAttribute<AcceptsAnchor> ();

                        if (accept_href != null)
                        {
                            stack.Pop ();

                            if (neighbor is PartialWhitespaceExpression.WhitespaceExpression)
                                stack.Pop ();

                            accept_href.Apply (_href, titleText: title);
                        }
                    }
                }
            }
        }

        public override string GenerateCode()
        {
            var extra = (no_follow ? " rel=\"nofollow\"" : "") + (open_new_window && !IgnoreOpenInNewWindow ? " target=\"_blank\"" : "").TrimStart(1);

            return string.Format ("<a href=\"{0}\"{3} title=\"{1}\">{2}</a>", _href, title, text != null ? text : _href, extra);
        }
    }

    public abstract class HyperLinkReference
    {
        readonly string href;

        static readonly Regex regex = new Regex (@"(?'scheme'\w+)://(?'host'(\w|\.|@)+(:\d+)?)(?'path'/([a-z]|[A-Z]|[0-9]|\(|\)|\+|%(([a-f]|[A-F])|\d){2}|~|\.|/|_|-|$|@|;|:|'|!|=|\?|&)+)?");
        static readonly None none = new None ();

        public static implicit operator string(HyperLinkReference href)
        {
            return href.href;
        }

        protected HyperLinkReference(string href)
        {
            this.href = href;
        }

        public static HyperLinkReference CreateHyperLink(string href)
        {
            return regex.IsMatch (href) ? (HyperLinkReference)new Some (href) : none;
        }

        sealed class Some : HyperLinkReference
        {
            public Some(string href) : base(href)
            {
                
            }
        }

        sealed class None : HyperLinkReference
        {
            public None() : base ("") { }
        }
    }
}