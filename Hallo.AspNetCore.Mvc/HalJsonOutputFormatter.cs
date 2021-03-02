using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hallo.Serialization;
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
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/hal+json"));
        }

        public override async Task WriteAsync(OutputFormatterWriteContext context)
        {
            await HalJsonGenerator.HalHandler(context.HttpContext, context.Object);
        }
    }
}