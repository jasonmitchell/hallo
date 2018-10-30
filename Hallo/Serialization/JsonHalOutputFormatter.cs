using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hallo.Serialization
{
    /// <summary>
    /// An output formatter for handling HAL+JSON response formatting
    /// </summary>
    /// <remarks>
    /// <p>
    /// This formatter supports the application/hal+json media type.
    /// </p>
    /// <p>
    /// When no HAL document generation has been implemented for the resource
    /// type being serialized it will fall back to the standard behaviour of
    /// <see cref="JsonOutputFormatter"/>
    /// </p>
    /// </remarks>
    /// <inheritdoc cref="JsonOutputFormatter"/>
    public class JsonHalOutputFormatter : JsonOutputFormatter
    {
        private const string ContentType = "application/hal+json";
        
        private static readonly JsonSerializerSettings DefaultSerializerSettings;

        static JsonHalOutputFormatter()
        {
            DefaultSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
            
            DefaultSerializerSettings.Converters.Add(new HalRepresentationConverter());
            DefaultSerializerSettings.Converters.Add(new LinksConverter());
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="JsonHalOutputFormatter"/> with
        /// default JSON serialization settings
        /// </summary>
        public JsonHalOutputFormatter()
            : this(DefaultSerializerSettings) {}
        
        /// <summary>
        /// Initializes a new instance of <see cref="JsonHalOutputFormatter"/>
        /// </summary>
        /// <param name="serializerSettings"><see cref="JsonSerializerSettings"/></param>
        public JsonHalOutputFormatter(JsonSerializerSettings serializerSettings)
            : this(serializerSettings, ArrayPool<char>.Shared) {}

        /// <summary>
        /// Initializes a new instance of <see cref="JsonHalOutputFormatter"/>
        /// </summary>
        /// <param name="serializerSettings"><see cref="JsonSerializerSettings"/></param>
        /// <param name="charPool"></param>
        public JsonHalOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool)
            : base(serializerSettings, charPool)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ContentType));
        }

        /// <inheritdoc cref="JsonOutputFormatter"/>
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var representationGenerator = GetRepresentationGenerator(context.HttpContext.RequestServices, context.ObjectType);
            if (representationGenerator == null)
            {
                await base.WriteResponseBodyAsync(context, selectedEncoding);
                return;
            }

            var representation = await representationGenerator.RepresentationOfAsync(context.Object);
            var json = JsonConvert.SerializeObject(representation, SerializerSettings);
            
            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            await response.WriteAsync(json);
        }

        private static IHal GetRepresentationGenerator(IServiceProvider services, Type resourceType)
        {
            var representationType = typeof(Hal<>).MakeGenericType(resourceType);
            var representation = (IHal)services.GetService(representationType);
            return representation;
        }
    }
}