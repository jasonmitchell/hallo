using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

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
    /// <see cref="SystemTextJsonOutputFormatter"/>
    /// </p>
    /// </remarks>
    /// <inheritdoc cref="SystemTextJsonOutputFormatter"/>
    public class HalJsonOutputFormatter : SystemTextJsonOutputFormatter
    {
        private const string ContentType = "application/hal+json";

        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        
        /// <summary>
        /// Initializes a new instance of <see cref="HalJsonOutputFormatter"/> with
        /// default JSON serialization settings
        /// </summary>
        public HalJsonOutputFormatter()
            : this(DefaultJsonSerializerOptions) {}
        
        /// <summary>
        /// Initializes a new instance of <see cref="HalJsonOutputFormatter"/>
        /// </summary>
        /// <param name="serializerOptions"><see cref="JsonSerializerOptions"/></param>
        public HalJsonOutputFormatter(JsonSerializerOptions serializerOptions)
            : base(serializerOptions)
        {
            serializerOptions.Converters.Add(new HalRepresentationConverter());
            serializerOptions.Converters.Add(new LinksConverter());

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ContentType));
        }

        public override async Task WriteAsync(OutputFormatterWriteContext context)
        {
            var representationGenerator = GetRepresentationGenerator(context.HttpContext.RequestServices, context.ObjectType);
            if (representationGenerator == null)
            {
                await WriteResponseBodyAsync(context, Encoding.UTF8);
                return;
            }

            var representation = await representationGenerator.RepresentationOfAsync(context.Object);
            var json = JsonSerializer.Serialize(representation, SerializerOptions);
            
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