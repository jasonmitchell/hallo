using System.Collections.Generic;

namespace Hallo.Test.Serialization.Supporting
{
    internal class DefaultRepresentation : Hal<DummyModel> { }

    internal class ModifiedStateRepresentation : Hal<DummyModel>,
                                                 IHalState<DummyModel>
    {
        public object StateFor(DummyModel resource)
        {
            return new
            {
                resource.Property
            };
        }
    }

    internal class FullRepresentation : Hal<DummyModel>,
                                        IHalState<DummyModel>,
                                        IHalEmbedded<DummyModel>,
                                        IHalLinks<DummyModel>
    {
        public object StateFor(DummyModel resource) => resource;

        public object EmbeddedFor(DummyModel resource)
        {
            return new
            {
                test = new
                {
                    Property = 1
                }
            };
        }

        public IEnumerable<Link> LinksFor(DummyModel resource)
        {
            yield return new Link(Link.Self, $"/dummy-model/{resource.Id}");
            yield return new Link("another-resource", $"/dummy-model/another-resource");
        }
    }

    internal class LinkedRepresentation : Hal<DummyModel>,
                                          IHalLinks<DummyModel>
    {
        public IEnumerable<Link> LinksFor(DummyModel resource)
        {
            yield return new Link(Link.Self, $"/dummy-model/{resource.Id}");
            yield return new Link("another-resource", $"/dummy-model/another-resource");
        }
    }

    internal class MultiLinkRelationRepresentation : Hal<DummyModel>,
                                                     IHalLinks<DummyModel>
    {
        public IEnumerable<Link> LinksFor(DummyModel resource)
        {
            yield return new Link(Link.Self, $"/dummy-model/{resource.Id}");
            yield return new Link(Link.Self, $"/something-else/{resource.Id}");
        }
    }
    
    internal class EmptyLinksRepresentation : Hal<DummyModel>,
                                              IHalLinks<DummyModel>
    {
        public IEnumerable<Link> LinksFor(DummyModel resource)
        {
            return new Link[0];
        }
    }

    internal class EmbeddedRepresentation : Hal<DummyModel>,
                                            IHalEmbedded<DummyModel>
    {
        public object EmbeddedFor(DummyModel resource)
        {
            return new
            {
                MagicNumber = 3
            };
        }
    }
    
    internal class DummyModelListRepresentation : PagedListRepresentation<DummyModel>
    {
        public DummyModelListRepresentation(IHalLinks<DummyModel> linkedRepresentation) 
            : base("/dummy-model", linkedRepresentation) { }
    }
    
    internal class CurieRepresentation : Hal<DummyModel>,
        IHalLinks<DummyModel>
    {
        public IEnumerable<Link> LinksFor(DummyModel resource)
        {
            yield return new Link("curies", "/docs/{rel}.html", "text/html", name: "foo", title: "Documentation",
                hrefLang: "en");
        }
    }
}