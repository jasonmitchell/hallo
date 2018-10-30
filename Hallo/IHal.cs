using System.Threading.Tasks;

namespace Hallo
{
    public interface IHal
    {
        /// <summary>
        /// Generates a <see cref="HalRepresentation"/> of the provided resource
        /// </summary>
        /// <param name="resource">The object to generate a <see cref="HalRepresentation"/> of</param>
        /// <returns><see cref="HalRepresentation"/></returns>
        Task<HalRepresentation> RepresentationOfAsync(object resource);
    }
}