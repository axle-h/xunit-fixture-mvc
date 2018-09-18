using System.Net.Http;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// Provides access to the HTTP request message to be used by the integration test.
    /// This is is registered with DI so is intended to be consumed via an implementation of <see cref="ITestServerBootstrap"/>.
    /// </summary>
    public class MvcFunctionalTestFixtureHttpRequestMessage
    {
        internal MvcFunctionalTestFixtureHttpRequestMessage(HttpRequestMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the HTTP request message to be used by the integration test.
        /// </summary>
        public HttpRequestMessage Message { get; }
    }
}