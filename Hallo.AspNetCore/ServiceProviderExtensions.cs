using System;

namespace Hallo.AspNetCore
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Tries to resolve a <see cref="Hal{TResource}"/> for the specified resource type.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="resourceType">The resource type to find a <see cref="Hal{TResource}"/> instance for</param>
        /// <returns>The resolved HAL representation generator or null</returns>
        public static IHal GetRepresentationGenerator(this IServiceProvider services, Type resourceType)
        {
            var representationType = typeof(Hal<>).MakeGenericType(resourceType);
            var representation = (IHal) services.GetService(representationType);
            return representation;
        }
    }
}