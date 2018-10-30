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
    public class JsonHalOutputFormatter : JsonOutputFormatter
    {
        private const string ContentType = "application/hal+json";
        
        private static readonly JsonSerializerSettings DefaultSerializerSettings = 
            JsonSerializerSettingsProvider.CreateSerializerSettings();

        static JsonHalOutputFormatter()
        {
            DefaultSerializerSettings.Converters.Add(new HalRepresentationConverter());
            DefaultSerializerSettings.Converters.Add(new LinksConverter());
        }
        
        public JsonHalOutputFormatter()
            : this(DefaultSerializerSettings, ArrayPool<char>.Shared) {}

        public JsonHalOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool)
            : base(serializerSettings, charPool)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ContentType));
        }

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