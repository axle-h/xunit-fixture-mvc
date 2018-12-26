using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
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
        /// Configures the specified bootstrap function to be:
        /// 1. Added to the test server DI container.
        /// 2. Resolved and run once the test server is constructed.
        /// </summary>
        /// <param name="bootstrapAction">The action to perform on the service provider during bootstrap.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingBootstrap(Func<IServiceProvider, Task> bootstrapAction);

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
        /// Configures the HTTP request message.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingConfiguredHttpMessage(Action<HttpRequestMessage> configurator);

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
        /// Adds an assertion that the response body should be successfully deserialized using the specified deserializer
        /// and then satisfy the specified assertion actions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture ShouldReturn<TResult>(Func<string, TResult> deserializer, params Action<TResult>[] assertions);

        /// <summary>
        /// Adds an assertion that will be run after the request has completed, resolving a service from DI.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture PostRequestResolvedServiceShould<TService>(Func<TService, Task> assertion)
            where TService : class;

        /// <summary>
        /// Sets the log minimum level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingMinimumLogLevel(LogLevel logLevel);

        /// <summary>
        /// Runs this fixture.
        /// </summary>
        /// <returns></returns>
        Task RunAsync();
    }
}