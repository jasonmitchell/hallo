using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Hallo.Test
{
    public class HalTests
    {
        [Fact]
        public void CreatesDefaultRepresentation()
        {
            var representation = new DefaultRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };

            var hal = ((IHal) representation).RepresentationOf(resource);
            hal.State.Should().BeEquivalentTo(resource);
        }

        [Fact]
        public void AppendsLinksToRepresentation()
        {
            var representation = new LinkedRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var hal = ((IHal) representation).RepresentationOf(resource);
            hal.State.Should().BeEquivalentTo(resource);
            hal.Links.Should().BeEquivalentTo(new[]
            {
                new Link("self", "/resource/123")
            });
        }

        [Fact]
        public void ModifiesRepresentationState()
        {
            var representation = new ModifiedStateRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var hal = ((IHal) representation).RepresentationOf(resource);
            hal.State.Should().BeEquivalentTo(new
            {
                A = 1
            });
        }

        [Fact]
        public void EmbedsResourceInRepresentation()
        {
            var representation = new EmbeddedRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var hal = ((IHal) representation).RepresentationOf(resource);
            hal.Embedded.Should().BeEquivalentTo(new
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