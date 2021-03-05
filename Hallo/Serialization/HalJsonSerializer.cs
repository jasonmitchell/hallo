using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hallo.Serialization
{
    public static class HalJsonSerializer
    {
        public static readonly JsonSerializerOptions DefaultSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            MaxDepth = 32,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new HalRepresentationConverter(),
                new LinksConverter()
            }
        };

        private static readonly Type[] RequiredConverters = 
        {
            typeof(HalRepresentationConverter),
            typeof(LinksConverter)
        };
        
        public static async Task<string> SerializeAsync(IHal representationGenerator, object resource, JsonSerializerOptions serializerOptions)
        {
            if (representationGenerator == null) throw new ArgumentNullException(nameof(representationGenerator));
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (serializerOptions == null) throw new ArgumentNullException(nameof(serializerOptions));

            if (!HalJsonConvertersPresent(serializerOptions))
            {
                var converters = string.Join(", ", RequiredConverters.Select(x => x.Name));
                throw new InvalidJsonSerializerOptionsException($"The JSON converters [{converters}] are required to serialize to hal+json");
            }

            var representation = await representationGenerator.RepresentationOfAsync(resource);
            var json = JsonSerializer.Serialize(representation, serializerOptions);
            
            return json;
        }

        private static bool HalJsonConvertersPresent(JsonSerializerOptions serializerOptions) =>
            RequiredConverters.All(x => serializerOptions.Converters.Any(y => y.GetType() == x));
    }
}