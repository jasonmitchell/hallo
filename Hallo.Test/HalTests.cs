using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Hallo.Test
{
    public class HalTests
    {
        [Fact]
        public async Task RepresentationOf_CreatesDefaultRepresentation()
        {
            var representation = new DefaultRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };

            var hal = await ((IHal) representation).RepresentationOfAsync(resource);
            hal.State.Should().BeEquivalentTo(resource);
        }

        [Fact]
        public async Task RepresentationOf_AppendsLinksToRepresentation()
        {
            var representation = new LinkedRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var hal = await ((IHal) representation).RepresentationOfAsync(resource);
            hal.State.Should().BeEquivalentTo(resource);
            hal.Links.Should().BeEquivalentTo(new[]
            {
                new Link("self", "/resource/123")
            });
        }

        [Fact]
        public async Task RepresentationOf_ModifiesRepresentationState()
        {
            var representation = new ModifiedStateRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var hal = await ((IHal) representation).RepresentationOfAsync(resource);
            hal.State.Should().BeEquivalentTo(new
            {
                A = 1
            });
        }

        [Fact]
        public async Task RepresentationOf_EmbedsResourceInRepresentation()
        {
            var representation = new EmbeddedRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var hal = await ((IHal) representation).RepresentationOfAsync(resource);
            hal.Embedded.Should().BeEquivalentTo(new
            {
                D = 123
            });
        }
        
        [Fact]
        public async Task LinksFor_ReturnsLinksForRepresentation()
        {
            var representation = new LinkedRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var links = await ((IHal) representation).LinksForAsync(resource);
            links.Should().BeEquivalentTo(new[]
            {
                new Link("self", "/resource/123")
            });
        }
        
//        [Fact]
//        public async Task StateFor_ReturnsStateForRepresentation()
//        {
//            var representation = new ModifiedStateRepresentation();
//            var resource = new ResourceModel
//            {
//                A = 1,
//                B = 2,
//                C = 3
//            };
//            
//            var state = await ((IHal) representation).StateForAsync(resource);
//            state.Should().BeEquivalentTo(new
//            {
//                A = 1
//            });
//        }
        
        [Fact]
        public async Task EmbeddedFor_ReturnsEmbeddedResourcesForRepresentation()
        {
            var representation = new EmbeddedRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var embedded = await ((IHal) representation).EmbeddedForAsync(resource);
            embedded.Should().BeEquivalentTo(new
            {
                D = 123
            });
        }
        
        private class ResourceModel
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
        }

        private class DefaultRepresentation : Hal<ResourceModel> { }

        private class LinkedRepresentation : Hal<ResourceModel>
        {
            protected override IEnumerable<Link> LinksFor(ResourceModel resource)
            {
                yield return new Link("self", "/resource/123");
            }
        }

        private class ModifiedStateRepresentation : Hal<ResourceModel>
        {
            protected override object StateFor(ResourceModel resource)
            {
                return new
                {
                    resource.A
                };
            }
        }

        private class EmbeddedRepresentation : Hal<ResourceModel>
        {
            protected override object EmbeddedFor(ResourceModel resource)
            {
                return new
                {
                    D = 123
                };
            }
        }
    }
}