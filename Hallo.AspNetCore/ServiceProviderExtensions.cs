using System;

namespace Hallo.AspNetCore
{
    public static class ServiceProviderExtensions
    {
        public static IHal GetRepresentationGenerator(this IServiceProvider services, Type resourceType)
        {
            var representationType = typeof(Hal<>).MakeGenericType(resourceType);
            var representation = (IHal) services.GetService(representationType);
            return representation;
        }
    }
}