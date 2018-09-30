using System;
using System.Text.RegularExpressions;

namespace Hal.Poc.Hypermedia
{
    public readonly struct Link
    {
        public const string Self = "self";
        
        private static readonly Regex IsTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);
        
        public string Rel { get; }
        public string Href { get; }
        public bool Templated => IsTemplatedRegex.IsMatch(Href);

        internal Link(string rel, string href)
        {
            if (string.IsNullOrWhiteSpace(rel))
            {
                throw new ArgumentException("Value is required", nameof(rel));
            }

            if (string.IsNullOrWhiteSpace(href))
            {
                throw new ArgumentException("Value is required", nameof(rel));
            }
            
            Rel = rel;
            Href = href;
        }
    }
}