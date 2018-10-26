using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hallo
{
    public class HalRepresentation
    {
        public object State { get; }
        
        [JsonProperty("_embedded")]
        public object Embedded { get; }
        
        [JsonProperty("_links")]
        public IEnumerable<Link> Links { get; }

        public HalRepresentation(object state, IEnumerable<Link> links)
        {
            State = state;
            Links = links;
        }

        public HalRepresentation(object state, object embedded, IEnumerable<Link> links)
        {
            State = state;
            Embedded = embedded;
            Links = links;
        }
    }
}