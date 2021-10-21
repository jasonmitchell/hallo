using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hallo.Serialization
{
    /// <summary>
    /// A derived <see cref="JsonConverter"/> which serializes a <see cref="IEnumerable{Link}"/> to JSON 
    /// </summary>
    public class LinksConverter : JsonConverter<IEnumerable<Link>>
    {
        /// <summary>
        /// NOT IMPLEMENTED: This converter does not support deserialization
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override IEnumerable<Link> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
            => throw new NotImplementedException();

        /// <inheritdoc cref="JsonConverter"/>
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
                        writer.WriteString("href", link.Href);
                        break;
                    case nameof(Link.Templated):
                        if (link.Templated)
                        {
                            writer.WriteBoolean("templated", true);
                        }
                        break;
                    case nameof(Link.Name):
                        if (link.Name != null)
                        {
                            writer.WriteString("name", link.Name);
                        }
                        break;
                    case nameof(Link.Deprecation):
                        if (link.Deprecation != null)
                        {
                            writer.WriteString("deprecation", link.Deprecation.ToString());
                        }
                        break;
                    case nameof(Link.Profile):
                        if (link.Profile != null)
                        {
                            writer.WriteString("profile", link.Profile.ToString());
                        }
                        break;
                    case nameof(Link.Title):
                        if (link.Title != null)
                        {
                            writer.WriteString("title", link.Title);
                        }
                        break;
                    case nameof(Link.Type):
                        if (link.Type != null)
                        {
                            writer.WriteString("type", link.Type);
                        }
                        break;
                    case nameof(Link.HrefLang):
                        if (link.HrefLang != null)
                        {
                            writer.WriteString("hreflang", link.HrefLang);
                        }
                        break;
                }
            }

            writer.WriteEndObject();
        }
    }
}
