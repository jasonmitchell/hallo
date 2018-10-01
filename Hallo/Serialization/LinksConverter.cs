using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Hallo.Serialization
{
    internal class LinksConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var links = ((IEnumerable<Link>) value).ToList();
            if (!links.Any())
            {
                return;
            }
            
            writer.WriteStartObject();

            foreach (var link in links)
            {
                // TODO: Link arrays
                writer.WritePropertyName(link.Rel);
                WriteLink(writer, link);
            }
            
            writer.WriteEndObject();
        }

        private void WriteLink(JsonWriter writer, in Link link)
        {
            writer.WriteStartObject();

            foreach (var info in link.GetType().GetProperties())
            {
                switch (info.Name)
                {
                    case nameof(Link.Href):
                        writer.WritePropertyName("href");
                        writer.WriteValue(link.Href);
                        break;
                    case nameof(Link.Templated):
                        if (link.Templated)
                        {
                            writer.WritePropertyName("templated");
                            writer.WriteValue(true);
                        }
                        break;
                }
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => reader.Value;
        
        public override bool CanConvert(Type objectType) 
            => typeof(IEnumerable<Link>).IsAssignableFrom(objectType);
    }
}