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
    public interface IMvcFunctionalTestFixture : ITestFixtureContext
    {
        /// <summary>
        /// Runs the specified fixture action during setup.
        /// This will do nothing if the fixture has already been setup.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingSetup(Action<IMvcFunctionalTestFixture> configurator);

        /// <summary>
        /// Configures the host test server to use the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingAspNetEnvironment(string environment);

        /// <summary>
        /// Configures the host test server configuration.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingConfiguration(Action<IConfigurationBuilder> action);

        /// <summary>
        /// Configures test server DI container services.
        /// </summary>
        /// <param name="servicesDelegate">The services delegate.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingServices(Action<IServiceCollection> servicesDelegate);

        /// <summary>
        /// Adds the specified configurator for the test server client.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingClientConfiguration(Action<WebApplicationFactoryClientOptions> configurator);

        /// <summary>
        /// Configured the logger factory that will be used by the test server and fixture.
        /// </summary>
        /// <param name="loggerConfigurator">The logger configurator.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingLogging(Action<ILoggingBuilder> loggerConfigurator);

        /// <summary>
        /// Adds a bootstrap action to the test fixture that will be run before the request.
        /// </summary>
        /// <param name="bootstrapFunction"></param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingBootstrap(Func<IServiceProvider, Task> bootstrapFunction);

        /// <summary>
        /// Updates the current test output helper.
        /// </summary>
        /// <param name="output">the test output helper.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture HavingTestOutput(ITestOutputHelper output);

        /// <summary>
        /// Configures the HTTP request message.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture When(Action<HttpRequestMessage> configurator);

        /// <summary>
        /// Adds assertions that will be run on the HTTP response.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        IMvcFunctionalTestFixture ShouldReturn(params Action<HttpResponseMessage>[] assertions);

        /// <summary>
        /// Adds an assertion that the response body should be successfully deserialized using the specified deserializer
        /// and then satisfy the specified assertion actions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture ShouldReturnBody<TResult>(Func<string, TResult> deserializer,
            params Action<TResult>[] assertions);

        /// <summary>
        /// Adds an assertion that the response body should be successfully deserialized using the specified deserializer
        /// and then satisfy the specified assertion functions, which have access to the service provider.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture ShouldReturnBody<TResult>(Func<string, TResult> deserializer,
            params Func<IServiceProvider, TResult, Task>[] assertions);

        /// <summary>
        /// Adds an assertion that will be run after the request has completed, resolving a service from DI.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture ShouldHaveServiceWhich(Func<IServiceProvider, Task> assertion);

        /// <summary>
        /// Adds an assertion that a further HTTP request will be satisfied by the specified configured fixture.
        /// </summary>
        /// <param name="contextFactory">The context function.</param>
        /// <param name="configurator">The fixture configurator.</param>
        /// <returns></returns>
        IMvcFunctionalTestFixture ShouldSatisfyRequest(Func<HttpResponseMessage, Task<object>> contextFactory,
            Action<object, IMvcFunctionalTestFixture> configurator);

        /// <summary>
        /// Builds this test fixture.
        /// </summary>
        /// <returns></returns>
        IMvcFunctionalTestFixture Build();

        /// <summary>
        /// Runs this fixture.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <returns></returns>
        Task RunAsync(ITestOutputHelper output = null);

        /// <summary>
        /// Gets the content root of the startup class.
        /// </summary>
        /// <returns></returns>
        string GetContentRoot();
    }
}