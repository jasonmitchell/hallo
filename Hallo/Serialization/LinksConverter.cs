using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hallo.Serialization
{
    public class LinksConverter : JsonConverter<IEnumerable<Link>>
    {
        public override IEnumerable<Link> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
            => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, IEnumerable<Link> value, JsonSerializerOptions options)
        {
            var links = value.GroupBy(x => x.Rel, x => x)
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
        
        private void WriteLinks(Utf8JsonWriter writer, in IEnumerable<Link> links)
        {
            writer.WriteStartArray();

            foreach (var link in links)
            {
                WriteLink(writer, link);
            }
            
            writer.WriteEndArray();
        }

        private void WriteLink(Utf8JsonWriter writer, in Link link)
        {
            writer.WriteStartObject();

            foreach (var info in link.GetType().GetProperties())
            {
                switch (info.Name)
                {
                    case nameof(Link.Href):
                        writer.WritePropertyName("href");
                        writer.WriteStringValue(link.Href);
                        break;
                    case nameof(Link.Templated):
                        if (link.Templated)
                        {
                            writer.WritePropertyName("templated");
                            writer.WriteBooleanValue(true);
                        }
                        break;
                }
            }

            writer.WriteEndObject();
        }
    }
}