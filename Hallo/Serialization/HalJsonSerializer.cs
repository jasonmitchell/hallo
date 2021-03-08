using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hallo.Serialization
{
    /// <summary>
    /// A class for handling serialization to hal+json
    /// </summary>
    public static class HalJsonSerializer
    {
        /// <summary>
        /// A default instance of <see cref="JsonSerializerOptions"/> which includes the required
        /// <see cref="JsonConverter"/> instances for serializing <see cref="HalRepresentation"/> types.
        /// </summary>
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
        
        /// <summary>
        /// Serializes a resource to a hal+json string using the provided <paramref name="representationGenerator"/>
        /// instance.
        /// </summary>
        /// <param name="representationGenerator">The <see cref="IHal"/> instance for generating a resource representation</param>
        /// <param name="resource">The object to serialize</param>
        /// <param name="serializerOptions"></param>
        /// <returns>A hal+json string</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidJsonSerializerOptionsException">Thrown if the provided <paramref name="serializerOptions"/> does not have the required converters</exception>
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