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
        public async Task AsyncImplementationTakesPrecedence()
        {
            var representation = new SyncAndAsyncRepresentation();
            var resource = new ResourceModel
            {
                A = 1,
                B = 2,
                C = 3
            };
            
            var hal = await ((IHal) representation).RepresentationOfAsync(resource);
            hal.State.Should().BeEquivalentTo(new
            {
                Source = "async"
            });
        }
        
        private class ResourceModel
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
        }

        private class DefaultRepresentation : Hal<ResourceModel> { }

        private class LinkedRepresentation : Hal<ResourceModel>, IHalLinks<ResourceModel>
        {
            public IEnumerable<Link> LinksFor(ResourceModel resource)
            {
                yield return new Link("self", "/resource/123");
            }
        }

        private class ModifiedStateRepresentation : Hal<ResourceModel>, IHalState<ResourceModel>
        {
            public object StateFor(ResourceModel resource)
            {
                return new
                {
                    resource.A
                };
            }
        }

        private class EmbeddedRepresentation : Hal<ResourceModel>, IHalEmbedded<ResourceModel>
        {
            public object EmbeddedFor(ResourceModel resource)
            {
                return new
                {
                    D = 123
                };
            }
        }

        private class SyncAndAsyncRepresentation : Hal<ResourceModel>,
                                                   IHalState<ResourceModel>,
                                                   IHalStateAsync<ResourceModel>
        {
            public object StateFor(ResourceModel resource)
            {
                return new
                {
                    Source = "sync"
                };
            }

            public Task<object> StateForAsync(ResourceModel resource)
            {
                return Task.FromResult<object>(new
                {
                    Source = "async"
                });
            }
        }
    }
}