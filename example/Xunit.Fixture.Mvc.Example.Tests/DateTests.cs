using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Fixture.Mvc.Example.Models;
using Xunit.Fixture.Mvc.Extensions;

namespace Xunit.Fixture.Mvc.Example.Tests
{
    public class DateTests
    {
        private readonly ITestOutputHelper _output;

        public DateTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public Task When_getting_date() =>
            new MvcFunctionalTestFixture<Startup>(_output)
               .WhenGetting("date")
               .ShouldReturnSuccessfulStatus()
               .ShouldReturnJson<DateDto>(x => x.UtcNow.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000))
               .RunAsync();
    }
}
