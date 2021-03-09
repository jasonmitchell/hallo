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
        /// A hint to indicate the media type expected when dereferencing the target resource
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// A URI that provides further information about the deprecation
        /// </summary>
        public Uri Deprecation { get; }

        /// <summary>
        /// A secondary key for selecting Links which share the same relation type
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A URI that hints about the profile of the target resource
        /// </summary>
        public Uri Profile { get; }

        /// <summary>
        /// A label for the link with a human-readable identifier
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The language of the target resource
        /// </summary>
        public string HrefLang { get; }

        /// <summary>
        /// Indicates if the hyperlink contains a URI template
        /// </summary>
        public bool Templated => IsTemplatedRegex.IsMatch(Href);

        /// <summary>
        /// Initializes a new instance of <see cref="Link"/>
        /// </summary>
        /// <param name="rel">The relation of the hyperlink</param>
        /// <param name="href">The URI or URI template of the hyperlink</param>
        /// <param name="type">A hint to indicate the media type expected when dereferencing the target resource</param>
        /// <param name="deprecation">A URI that provides further information about the deprecation</param>
        /// <param name="name">A secondary key for selecting Links which share the same relation type</param>
        /// <param name="profile">A URI that hints about the profile of the target resource</param>
        /// <param name="title">A label for the link with a human-readable identifier</param>
        /// <param name="hrefLang">The language of the target resource</param>
        public Link(string rel, string href, string type = null, Uri deprecation = null, string name = null,
            Uri profile = null, string title = null, string hrefLang = null)
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
            Type = type;
            Deprecation = deprecation;
            Name = name;
            Profile = profile;
            Title = title;
            HrefLang = hrefLang;
        }
    }
}