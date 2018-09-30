namespace Hal.Poc.Hypermedia
{
    internal interface IHal
    {
        HalRepresentation RepresentationOf(object resource);
    }
}