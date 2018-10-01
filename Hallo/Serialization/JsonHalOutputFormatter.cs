using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hallo.Serialization
{
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

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var representationGenerator = GetRepresentationGenerator(context.HttpContext.RequestServices, context.ObjectType);
            if (representationGenerator == null)
            {
                return base.WriteResponseBodyAsync(context, selectedEncoding);
            }

            var representation = representationGenerator.RepresentationOf(context.Object);
            
            var jObject = JObject.FromObject(representation, CreateJsonSerializer());
            var state = jObject.Property("state");
            jObject.Add(state.Value.Children<JProperty>());
            state.Remove();
            
            var json = JsonConvert.SerializeObject(jObject, SerializerSettings);
            
            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            return response.WriteAsync(json);
        }

        private static IHal GetRepresentationGenerator(IServiceProvider services, Type resourceType)
        {
            var representationType = typeof(Hal<>).MakeGenericType(resourceType);
            var representation = (IHal)services.GetService(representationType);
            return representation;
        }
    }
}