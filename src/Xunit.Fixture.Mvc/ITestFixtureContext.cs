using System.Collections.Generic;
using System.Net.Http;
using AutoFixture;
using Bogus;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// Context for the MVC functional test fixture.
    /// This is registered in DI so can be resolved by a bootstrap step or service provider assertion.
    /// </summary>
    public interface ITestFixtureContext
    {
        /// <summary>
        /// Gets the auto fixture.
        /// </summary>
        /// <value>
        /// The auto fixture.
        /// </value>
        IFixture AutoFixture { get; }

        /// <summary>
        /// Gets the faker.
        /// </summary>
        Faker Faker { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Gets the configured logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the current HTTP request message.
        /// Or <c>null</c> if this fixture does not yet have a act step.
        /// </summary>
        HttpRequestMessage RequestMessage { get; }

        /// <summary>
        /// Gets the current test output helper.
        /// Or <c>null</c> if this fixture has not yet been associated with a test.
        /// </summary>
        ITestOutputHelper TestOutput { get; }
    }
}