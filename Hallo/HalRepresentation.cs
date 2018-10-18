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

        internal HalRepresentation(object state, object embedded, IEnumerable<Link> links)
        {
            State = state;
            Embedded = embedded;
            Links = links;
        }

        public HalRepresentation WithoutEmbedded()
        {
            return new HalRepresentation(State, null, Links);
        }
    }
}