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
            var links = ((IEnumerable<Link>) value).GroupBy(x => x.Rel, x => x)
                                                   .ToDictionary(x => x.Key, x => x.ToList());
            
            if (!links.Any())
            {
                return;
            }
            
            writer.WriteStartObject();

            foreach (var link in links)
            {
                writer.WritePropertyName(link.Key);
                
                if (link.Value.Count > 1)
                {
                    WriteLinks(writer, link.Value);
                }
                else
                {
                    WriteLink(writer, link.Value.Single());
                }
            }
            
            writer.WriteEndObject();
        }

        private void WriteLinks(JsonWriter writer, in IEnumerable<Link> links)
        {
            writer.WriteStartArray();

            foreach (var link in links)
            {
                WriteLink(writer, link);
            }
            
            writer.WriteEndArray();
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