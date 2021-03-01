using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hallo.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Hallo.AspNetCore.Mvc
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

        /// <summary>
        /// Initializes a new instance of <see cref="HalJsonOutputFormatter"/> with
        /// default JSON serialization settings
        /// </summary>
        public HalJsonOutputFormatter()
            : this(HalJsonSerializer.DefaultSerializerOptions)
        {
        }

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
            var json = await HalJsonGenerator.GenerateHalJson(context.HttpContext, context.Object, SerializerOptions);
            if (json == null)
            {
                context.HttpContext.Response.ContentType = "application/json";
                await WriteResponseBodyAsync(context, Encoding.UTF8);
                return;
            }

            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            await response.WriteAsync(json);
        }
    }
}