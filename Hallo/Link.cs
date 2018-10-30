using System;
using System.Text.RegularExpressions;

namespace Hallo
{
    /// <summary>
    /// A struct for defining a hyperlink from the containing resource to a URI
    /// </summary>
    public readonly struct Link
    {
        /// <summary>
        /// The "self" relation
        /// </summary>
        public const string Self = "self";
        
        private static readonly Regex IsTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);
        
        /// <summary>
        /// The relation of the hyperlink to the containing resource
        /// </summary>
        public string Rel { get; }
        
        /// <summary>
        /// The URI or URI template of the hyperlink
        /// </summary>
        public string Href { get; }
        
        /// <summary>
        /// Indicates if the hyperlink contains a URI template
        /// </summary>
        public bool Templated => IsTemplatedRegex.IsMatch(Href);

        /// <summary>
        /// Initializes a new instance of <see cref="Link"/>
        /// </summary>
        /// <param name="rel">The relation of the hyperlink</param>
        /// <param name="href">The URI or URI template of the hyperlink</param>
        public Link(string rel, string href)
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