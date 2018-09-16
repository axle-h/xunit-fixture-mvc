using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// A functional test fixture for MVC.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IMvcFunctionalTestFixture
    {
        /// <summary>
        /// Gets the auto fixture.
        /// </summary>
        /// <value>
        /// The auto fixture.
        /// </value>
        AutoFixture.Fixture AutoFixture { get; }

        /// <summary>
        /// Configures the host test server to use the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingAspNetEnvironment(string environment);

        /// <summary>
        /// Configures the host test server configuration.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingConfiguration(Action<ITestOutputHelper, IConfigurationBuilder> action);

        /// <summary>
        /// Configures an instance of the specified bootstrap service to be:
        /// 1. Added to the test server DI container.
        /// 2. Resolved and run once the test server is constructed.
        /// </summary>
        /// <typeparam name="TTestDataBootstrapService">The type of the test data bootstrap.</typeparam>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingBootstrap<TTestDataBootstrapService>()
            where TTestDataBootstrapService : class, ITestServerBootstrap;

        /// <summary>
        /// Configures test server DI container services.
        /// </summary>
        /// <param name="servicesDelegate">The services delegate.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingServices(Action<IServiceCollection> servicesDelegate);

        /// <summary>
        /// Adds the specified configurator for the test server client.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingClientConfiguration(Action<WebApplicationFactoryClientOptions> configurator);

        /// <summary>
        /// Configures the request to use the specified string as a bearer token in the authorization header.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingBearerToken(string token);

        /// <summary>
        /// Configures the fixture perform the specified HTTP action.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The HTTP content.</param>
        IMvcFunctionalTestFixture When(HttpMethod method, string uri, HttpContent content);

        /// <summary>
        /// Adds assertions that will be run on the HTTP response.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        IMvcFunctionalTestFixture ResponseShould(params Action<HttpResponseMessage>[] assertions);

        /// <summary>
        /// Adds assertions that will be run on the HTTP, JSON response body.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="assertions">The assertions.</param>
        IMvcFunctionalTestFixture ShouldReturnJson<TResult>(params Action<TResult>[] assertions);

        /// <summary>
        /// Adds an assertion that will be run after the request has completed, resolving a service from DI.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture PostRequestResolvedServiceShould<TService>(Func<TService, Task> assertion)
            where TService : class;

        /// <summary>
        /// Runs this fixture.
        /// </summary>
        /// <returns></returns>
        Task RunAsync();

        /// <summary>
        /// Sets the log minimum level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingMinimumLogLevel(LogLevel logLevel);
    }
}