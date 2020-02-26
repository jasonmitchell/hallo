using System;
using FluentAssertions;
using Xunit;

namespace Hallo.Test
{
    public class LinkTests
    {
        [Fact]
        public void PopulatesLink()
        {
            var link = new Link("self", "/href");
            link.Rel.Should().Be("self");
            link.Href.Should().Be("/href");
            link.Templated.Should().BeFalse();
        }

        [Fact]
        public void Equatable()
        {
            var linkA = new Link("self", "/href", "application/json", new UriBuilder().Uri, "A Link", new UriBuilder().Uri, "A Link Title", "en-IE");
            var linkB = new Link("self", "/href", "application/json", new UriBuilder().Uri, "A Link", new UriBuilder().Uri, "A Link Title", "en-IE");

            linkA.Equals(linkB).Should().Be(true);
        }
        
        [Theory, InlineData(null), InlineData(""), InlineData("  ")]
        public void RelIsRequired(string rel)
        {
            Action action = () => new Link(rel, "/href");
            action.Should().Throw<ArgumentException>();
        }
        
        [Theory, InlineData(null), InlineData(""), InlineData("  ")]
        public void HrefIsRequired(string href)
        {
            Action action = () => new Link("self", href);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void TemplatedIsTrueWhenHrefContainsPlaceholder()
        {
            var link = new Link("self", "/href/{template}");
            link.Templated.Should().BeTrue();
        }
    }
}