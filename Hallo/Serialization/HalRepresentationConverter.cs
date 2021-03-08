using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hallo.Serialization
{
    /// <summary>
    /// A derived <see cref="JsonConverter"/> which serializes a <see cref="HalRepresentation"/> to JSON 
    /// </summary>
    public class HalRepresentationConverter : JsonConverter<HalRepresentation>
    {
        /// <inheritdoc cref="JsonConverter"/>
        public override bool CanConvert(Type objectType) 
            => typeof(HalRepresentation).IsAssignableFrom(objectType);

        /// <summary>
        /// NOT IMPLEMENTED: This converter does not support deserialization
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override HalRepresentation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
            => throw new NotImplementedException();

        /// <inheritdoc cref="JsonConverter"/>
        public override void Write(Utf8JsonWriter writer, HalRepresentation value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            WriteState(writer, value.State, options);
            WriteEmbedded(writer, value.Embedded, options);
            WriteLinks(writer, value.Links, options);
            writer.WriteEndObject();
        }

        private static void WriteState(Utf8JsonWriter writer, object state, JsonSerializerOptions options) 
            => WriteObjectProperties(writer, state, options);
        
        private static void WriteEmbedded(Utf8JsonWriter writer, object? embedded, JsonSerializerOptions options)
        {
            if (embedded == null)
            {
                return;
            }
            
            writer.WriteStartObject("_embedded");
            WriteObjectProperties(writer, embedded, options);
            writer.WriteEndObject();
        }

        private static void WriteLinks(Utf8JsonWriter writer, IEnumerable<Link> links, JsonSerializerOptions options)
        {
            if (links == null || !links.Any())
            {
                return;
            }
            
            writer.WritePropertyName("_links");
            var linkConverter = (LinksConverter) options.GetConverter(typeof(IEnumerable<Link>));
            linkConverter.Write(writer, links, options);
        }

        private static void WriteObjectProperties(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var json = JsonDocument.Parse(JsonSerializer.Serialize(value, options));
            foreach (var jsonProperty in json.RootElement.EnumerateObject()) 
                jsonProperty.WriteTo(writer);
        }
    }
}