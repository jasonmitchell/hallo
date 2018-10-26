using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hallo.Serialization
{
    internal class HalRepresentationConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var representation = (HalRepresentation) value;
            var root = new JObject();
            
            ConvertState(representation.State, root, serializer);
            ConvertEmbedded(representation.Embedded, root, serializer);
            ConvertLinks(representation.Links, root, serializer);

            root.WriteTo(writer);
        }

        private static void ConvertState(object state, JObject node, JsonSerializer serializer)
        {
            if (state == null)
            {
                return;
            }

            var stateNode  = JObject.FromObject(state, serializer);
            node.Add(stateNode.Children<JProperty>());
        }
        
        private static void ConvertEmbedded(object embedded, JObject node, JsonSerializer serializer)
        {
            if (embedded == null)
            {
                return;
            }

            var embeddedNode = JObject.FromObject(embedded, serializer);
            node.Add("_embedded", embeddedNode);
        }
        
        private static void ConvertLinks(IEnumerable<Link> links, JObject node, JsonSerializer serializer)
        {
            if (links == null)
            {
                return;
            }

            var linksNode = JObject.FromObject(links, serializer);
            node.Add("_links", linksNode);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => reader.Value;
        
        public override bool CanConvert(Type objectType) 
            => typeof(HalRepresentation).IsAssignableFrom(objectType);
    }
}