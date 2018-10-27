using System.Threading.Tasks;

namespace Hallo
{
    internal interface IHal
    {
        Task<HalRepresentation> RepresentationOfAsync(object resource);
    }
}