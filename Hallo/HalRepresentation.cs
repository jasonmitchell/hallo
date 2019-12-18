using System.Collections.Generic;

namespace Hallo
{
    /// <summary>
    /// A class for defining a generated HAL document for a resource
    /// </summary>
    public class HalRepresentation
    {
        /// <summary>
        /// The state of the requested resource
        /// </summary>
        public object State { get; }
        
        /// <summary>
        /// The additional resources to be embedded in the document
        /// </summary>
        public object? Embedded { get; }
        
        /// <summary>
        /// The hyperlinks to related resources
        /// </summary>
        public IEnumerable<Link> Links { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HalRepresentation"/>
        /// </summary>
        /// <param name="state">The state of the requested resource</param>
        /// <param name="links">The hyperlinks to related resources</param>
        public HalRepresentation(object state, IEnumerable<Link> links)
        {
            State = state;
            Links = links;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HalRepresentation"/>
        /// </summary>
        /// <param name="state">The state of the requested resource</param>
        /// <param name="embedded">The additional resources to be embedded in the document</param>
        /// <param name="links">The hyperlinks to related resources</param>
        public HalRepresentation(object state, object? embedded, IEnumerable<Link> links)
        {
            State = state;
            Embedded = embedded;
            Links = links;
        }
    }
}