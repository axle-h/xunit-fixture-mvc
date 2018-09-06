using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Xunit.Abstractions;
using Xunit.Fixture.Mvc.Infrastructure;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// A functional test fixture for MVC
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup.</typeparam>
    /// <seealso cref="IMvcFunctionalTestFixture" />
    public class MvcFunctionalTestFixture<TStartup> : IMvcFunctionalTestFixture
        where TStartup : class
    {
        private readonly ITestOutputHelper _output;
        private readonly IServiceCollection _extraServices = new ServiceCollection();
        private readonly IList<Action<ITestOutputHelper, IConfigurationBuilder>> _configurationBuilderDelegates = new List<Action<ITestOutputHelper, IConfigurationBuilder>>();
        private readonly IList<Action<WebApplicationFactoryClientOptions>> _clientConfigurationDelegates = new List<Action<WebApplicationFactoryClientOptions>>();
        private readonly IList<Action<HttpResponseMessage>> _responseAssertions = new List<Action<HttpResponseMessage>>();
        private readonly IList<Action<object>> _resultAssertions = new List<Action<object>>();
        private readonly IList<(Type serviceType, Func<object, Task> assertion)> _postRequestAssertions = new List<(Type serviceType, Func<object, Task> assertion)>();
        private readonly HttpRequestMessage _message = new HttpRequestMessage();

        private bool _actStepConfigured;
        private Type _resultType;
        private string _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcFunctionalTestFixture{TStartup}"/> class.
        /// </summary>
        /// <param name="testOutputHelper">The test output helper.</param>
        public MvcFunctionalTestFixture(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        /// <summary>
        /// Configures the host test server to use the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingAspNetEnvironment(string environment)
        {
            _environment = environment;
            return this;
        }

        /// <summary>
        /// Configures the host test server configuration.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingConfiguration(Action<ITestOutputHelper, IConfigurationBuilder> action)
        {
            _configurationBuilderDelegates.Add(action);
            return this;
        }

        /// <summary>
        /// Configures an instance of the specified bootstrap service to be:
        /// 1. Added to the test server DI container.
        /// 2. Resolved and run once the test server is constructed.
        /// </summary>
        /// <typeparam name="TTestDataBootstrapService">The type of the test data bootstrap.</typeparam>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingBootstrap<TTestDataBootstrapService>()
            where TTestDataBootstrapService : class, ITestServerBootstrap =>
            HavingServices(services => services.AddScoped<ITestServerBootstrap, TTestDataBootstrapService>());

        /// <summary>
        /// Configures test server DI container services.
        /// </summary>
        /// <param name="servicesDelegate">The services delegate.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingServices(Action<IServiceCollection> servicesDelegate)
        {
            servicesDelegate(_extraServices);
            return this;
        }

        /// <summary>
        /// Adds the specified configurator for the test server client.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingClientConfiguration(Action<WebApplicationFactoryClientOptions> configurator)
        {
            _clientConfigurationDelegates.Add(configurator);
            return this;
        }

        /// <summary>
        /// Configures the request to use the specified string as a bearer token in the authorization header.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingBearerToken(string token)
        {
            _message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }
        
        /// <summary>
        /// Configures the fixture perform the specified HTTP action.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The HTTP content.</param>
        public IMvcFunctionalTestFixture When(HttpMethod method, string uri, HttpContent content)
        {
            _actStepConfigured = true;

            _message.Method = method;
            _message.RequestUri = new Uri(uri, UriKind.Relative);
            _message.Content = content;
            return this;
        }

        /// <summary>
        /// Adds assertions that will be run on the HTTP response.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        public IMvcFunctionalTestFixture ResponseShould(params Action<HttpResponseMessage>[] assertions)
        {
            foreach (var assertion in assertions)
            {
                _responseAssertions.Add(assertion);
            }

            return this;
        }

        /// <summary>
        /// Adds assertions that will be run on the HTTP, JSON response body.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="assertions">The assertions.</param>
        /// <exception cref="ArgumentException">TResult</exception>
        public IMvcFunctionalTestFixture JsonResultShould<TResult>(params Action<TResult>[] assertions)
        {
            if (_resultType != null && _resultType != typeof(TResult))
            {
                throw new ArgumentException($"Already added JSON assertions for {_resultType}", nameof(TResult));
            }

            // This implicitly adds an assertion that the result is a valid JSON representation of TResult.
            _resultType = typeof(TResult);

            foreach (var assertion in assertions)
            {
                _resultAssertions.Add(o => assertion(o.Should().BeAssignableTo<TResult>().Which));
            }

            return this;
        }

        /// <summary>
        /// Adds an assertion that will be run after the request has completed, resolving a service from DI.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture PostRequestResolvedServiceShould<TService>(Func<TService, Task> assertion)
            where TService : class
        {
            _postRequestAssertions.Add((typeof(TService), o => assertion((TService) o)));
            return this;
        }

        /// <summary>
        /// Gets the auto fixture.
        /// </summary>
        /// <value>
        /// The auto fixture.
        /// </value>
        public AutoFixture.Fixture AutoFixture { get; } = new AutoFixture.Fixture();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => RunAsync().Wait();

        private async Task RunAsync()
        {
            if (!_actStepConfigured)
            {
                throw new InvalidOperationException($"Must call {nameof(When)} to configure an HTTP request");
            }
            
            using (var loggerProvider = _output == null ? NullLoggerProvider.Instance as ILoggerProvider : new TestOutputHelperLoggerProvider(_output))
            using (var factory = new FixtureWebApplicationFactory(_output, loggerProvider, _environment, _extraServices, _configurationBuilderDelegates, _clientConfigurationDelegates))
            using (var client = factory.CreateClient()) // this actually builds the test server.
            {
                

                var logger = loggerProvider.CreateLogger(GetType().ToString());

                // Bootstrap.
                using (var scope = factory.Server.Host.Services.CreateScope())
                {
                    foreach (var bootstrap in scope.ServiceProvider.GetService<IEnumerable<ITestServerBootstrap>>())
                    {
                        logger.LogInformation($"Bootstrapping {bootstrap.GetType()}");
                        await bootstrap.BootstrapAsync();
                    }
                }

                using (var aggregator = new ExceptionAggregator())
                {
                    logger.LogInformation($"Sending request {_message}");
                    var response = await client.SendAsync(_message);

                    // Response assertions.
                    foreach (var assertion in _responseAssertions)
                    {
                        aggregator.Try(() => assertion(response));
                    }

                    // Response body (result) assertions.
                    if (_resultType != null)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        logger.LogInformation("Received: " + responseBody);

                        try
                        {
                            var result = JsonConvert.DeserializeObject(responseBody, _resultType);
                            foreach (var assertion in _resultAssertions)
                            {
                                aggregator.Try(() => assertion(result));
                            }
                        }
                        catch (JsonException e)
                        {
                            aggregator.Add(e);
                        }
                    }
                    else
                    {
                        logger.LogInformation("No result assertions set, ignoring response body");
                    }
                    
                    // Post request assertions.
                    using (var scope = factory.Server.Host.Services.CreateScope())
                    {
                        foreach (var (serviceType, assertion) in _postRequestAssertions)
                        {
                            logger.LogInformation($"Running post request assertion on service: {serviceType}");
                            var service = scope.ServiceProvider.GetRequiredService(serviceType);
                            await aggregator.TryAsync(() => assertion(service));
                        }
                    }
                }
            }
            
        }

        private class FixtureWebApplicationFactory : WebApplicationFactory<TStartup>
        {
            private readonly ITestOutputHelper _output;
            private readonly ILoggerProvider _loggerProvider;
            private readonly string _environment;
            private readonly IServiceCollection _extraServices;
            private readonly IEnumerable<Action<ITestOutputHelper, IConfigurationBuilder>> _configurationBuilderDelegates;

            public FixtureWebApplicationFactory(ITestOutputHelper output,
                                                               ILoggerProvider loggerProvider,
                                                               string environment,
                                                               IServiceCollection extraServices,
                                                               IEnumerable<Action<ITestOutputHelper, IConfigurationBuilder>> configurationBuilderDelegates,
                                                               IEnumerable<Action<WebApplicationFactoryClientOptions>> clientConfigurationDelegates)
            {
                _output = output;
                _loggerProvider = loggerProvider;
                _environment = environment;
                _extraServices = extraServices;
                _configurationBuilderDelegates = configurationBuilderDelegates;


                foreach (var configurator in clientConfigurationDelegates)
                {
                    configurator(ClientOptions);
                }
            }

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureAppConfiguration(configurationBuilder =>
                                                  {
                                                      foreach (var action in _configurationBuilderDelegates)
                                                      {
                                                          action(_output, configurationBuilder);
                                                      }
                                                  })
                       .UseEnvironment(_environment ?? EnvironmentName.Production)
                       .ConfigureLogging(b => b.AddProvider(_loggerProvider))
                       .ConfigureServices(services =>
                                          {
                                              foreach (var service in _extraServices)
                                              {
                                                  services.Add(service);
                                              }
                                          });
            }
        }
    }
}