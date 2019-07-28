using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Xunit.Fixture.Mvc.Example.Models;
using Xunit.Fixture.Mvc.Extensions;

namespace Xunit.Fixture.Mvc.Example.Tests
{
    /// <summary>
    /// Demonstrates sharing an MVC functional test fixture across multiple tests.
    /// </summary>
    /// <seealso cref="Xunit.Fixture.Mvc.MvcFunctionalTestBase{Startup}" />
    public class DateTests : MvcFunctionalTestBase<Startup>
    {
        private const string FutureOffset = "future_offset";

        public DateTests(MvcFunctionalTestClassFixture<Startup> fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        protected override IMvcFunctionalTestFixture GivenFixtureSetup(IMvcFunctionalTestFixture fixture) =>
            fixture
                .HavingFake(f => f.Date.Timespan(), out var futureOffset)
                .HavingProperty(FutureOffset, futureOffset)
                .HavingConfiguration(b => b.AddInMemoryCollection(new Dictionary<string, string>
                {
                    [FutureOffset] = futureOffset.ToString()
                }));

        [Fact]
        public Task When_getting_date_and_getting_date_again() =>
            GivenClassFixture()
               .WhenGetting("date")
               .ShouldReturnSuccessfulStatus()
               .ShouldReturnJson<DateDto>(x => x.UtcNow.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000))
               .ShouldSatisfyJsonRequest<DateDto>((r, f) => f
                   .WhenGetting("date")
                   .ShouldReturnSuccessfulStatus()
                   .ShouldReturnJson<DateDto>(x => x.UtcNow.Should().BeAfter(r.UtcNow)))
               .RunAsync();

        [Fact]
        public Task When_getting_future_date() =>
            GivenClassFixture()
                .HavingConfiguredProperty(FutureOffset, out TimeSpan futureOffset)
                .WhenGetting("date/future")
                .ShouldReturnSuccessfulStatus()
                .ShouldReturnJson<DateDto>(x => x.UtcNow.Should().BeCloseTo(DateTimeOffset.UtcNow.Date.Add(futureOffset), 1000))
                .RunAsync();
    }


}
