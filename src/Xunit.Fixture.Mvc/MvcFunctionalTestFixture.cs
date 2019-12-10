using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Fixture.Mvc.Infrastructure;
using Xunit.Sdk;
using HostingHostBuilderExtensions = Microsoft.Extensions.Hosting.HostingHostBuilderExtensions;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// A functional test fixture for MVC.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup.</typeparam>
    /// <seealso cref="IMvcFunctionalTestFixture" />
    public class MvcFunctionalTestFixture<TStartup> : WebApplicationFactory<TStartup>, IMvcFunctionalTestFixture
        where TStartup : class
    {
        private readonly IServiceCollection _extraServices = new ServiceCollection();

        private readonly IList<Action<IConfigurationBuilder>> _configurationBuilders = new List<Action<IConfigurationBuilder>>();

        private readonly IList<Action<ILoggingBuilder>> _loggerConfigurators = new List<Action<ILoggingBuilder>>();

        private readonly ICollection<Func<IServiceProvider, Task>> _bootstrapFunctions = new List<Func<IServiceProvider, Task>>();

        private readonly IList<Assertion> _assertions = new List<Assertion>();

        private readonly TestOutputHelperLoggerProvider _loggerProvider;
        private readonly WebApplicationFactory<TStartup> _parent;

        private bool _built;
        private bool _run;
        private string _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcFunctionalTestFixture{TStartup}"/> class.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        public MvcFunctionalTestFixture(ITestOutputHelper output) : this(output, null)
        {
        }

        private WebApplicationFactory<TStartup> Factory => _parent ?? this;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcFunctionalTestFixture{TStartup}"/> class.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <param name="sink">The message sink.</param>
        internal MvcFunctionalTestFixture(ITestOutputHelper output, IMessageSink sink)
        {
            _loggerProvider = new TestOutputHelperLoggerProvider(sink, output);
            HavingServices(services => services.AddSingleton<ITestFixtureContext>(this));
        }

        private MvcFunctionalTestFixture(TestOutputHelperLoggerProvider loggerProvider,
            WebApplicationFactory<TStartup> parent)
        {
            _loggerProvider = loggerProvider;
            _parent = parent;
        }
        
        /// <summary>
        /// Gets the auto fixture.
        /// </summary>
        /// <value>
        /// The auto fixture.
        /// </value>
        public IFixture AutoFixture { get; } = new AutoFixture.Fixture();

        /// <summary>
        /// Gets the faker.
        /// </summary>
        public Faker Faker { get; } = new Faker();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the configured logger.
        /// </summary>
        public ILogger Logger => _loggerProvider.CreateLogger(typeof(MvcFunctionalTestFixture<TStartup>).Name);

        /// <summary>
        /// Gets the current HTTP request message.
        /// </summary>
        public HttpRequestMessage RequestMessage { get; private set; }

        /// <summary>
        /// Gets the current test output helper.
        /// Or <c>null</c> if this fixture has not yet been associated with a test.
        /// </summary>
        public ITestOutputHelper TestOutput => _loggerProvider.TestOutput;

        /// <summary>
        /// Runs the specified fixture action during setup.
        /// This will do nothing if the fixture has already been setup.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingSetup(Action<IMvcFunctionalTestFixture> configurator)
        {
            if (!_built)
            {
                configurator(this);
            }

            return this;
        }

        /// <summary>
        /// Configures the host test server to use the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingAspNetEnvironment(string environment) =>
            WithSetup(() => _environment = environment);

        /// <summary>
        /// Configures the host test server configuration.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingConfiguration(Action<IConfigurationBuilder> action) =>
            WithSetup(() => _configurationBuilders.Add(action));

        /// <summary>
        /// Configures test server DI container services.
        /// </summary>
        /// <param name="servicesDelegate">The services delegate.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingServices(Action<IServiceCollection> servicesDelegate) =>
            WithSetup(() => servicesDelegate(_extraServices));

        /// <summary>
        /// Adds the specified configurator for the test server client.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingClientConfiguration(Action<WebApplicationFactoryClientOptions> configurator) =>
            WithSetup(() => configurator(ClientOptions));

        /// <summary>
        /// Configured the logger factory that will be used by the test server and fixture.
        /// </summary>
        /// <param name="loggerConfigurator">The logger configurator.</param>
        /// <exception cref="InvalidOperationException">When the fixture has already been built.</exception>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingLogging(Action<ILoggingBuilder> loggerConfigurator) =>
            WithSetup(() => _loggerConfigurators.Add(loggerConfigurator));

        /// <summary>
        /// Updates the current test output helper.
        /// </summary>
        /// <param name="output">the test output helper.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingTestOutput(ITestOutputHelper output)
        {
            _loggerProvider.SetTestOutputHelper(output);
            return this;
        }

        /// <summary>
        /// Adds a bootstrap action to the test fixture that will be run before the request.
        /// </summary>
        /// <param name="bootstrapFunction"></param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture HavingBootstrap(Func<IServiceProvider, Task> bootstrapFunction)
        {
            _bootstrapFunctions.Add(bootstrapFunction);
            return this;
        }

        /// <summary>
        /// Configures the HTTP request message.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture When(Action<HttpRequestMessage> configurator)
        {
            _run = false;

            if (RequestMessage == null)
            {
                RequestMessage = new HttpRequestMessage();
            }

            configurator(RequestMessage);

            return this;
        }

        /// <summary>
        /// Adds assertions that will be run on the HTTP response.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        public IMvcFunctionalTestFixture ShouldReturn(params Action<HttpResponseMessage>[] assertions)
        {
            foreach (var assertion in assertions)
            {
                _assertions.Add(new ResponseAssertion { Assertion = assertion });
            }

            return this;
        }

        /// <summary>
        /// Adds an assertion that the response body should be successfully deserialized using the specified deserializer
        /// and then satisfy the specified assertion actions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture ShouldReturnBody<TResult>(Func<string, TResult> deserializer, params Action<TResult>[] assertions)
        {
            _assertions.Add(new ResponseBodyAssertion
            {
                Deserializer = x => deserializer(x),
                Assertions = assertions.Select<Action<TResult>, Func<IServiceProvider, object, Task>>(a =>
                    (sp, o) =>
                    {
                        a((TResult) o);
                        return Task.CompletedTask;
                    }).ToList()
            });
            return this;
        }

        /// <summary>
        /// Adds an assertion that the response body should be successfully deserialized using the specified deserializer
        /// and then satisfy the specified assertion functions, which have access to the service provider.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture ShouldReturnBody<TResult>(Func<string, TResult> deserializer, params Func<IServiceProvider, TResult, Task>[] assertions)
        {
            _assertions.Add(new ResponseBodyAssertion
            {
                Deserializer = x => deserializer(x),
                Assertions = assertions
                    .Select<Func<IServiceProvider, TResult, Task>, Func<IServiceProvider, object, Task>>(a =>
                        (sp, o) => a(sp, (TResult) o)).ToList()
            });
            return this;
        }

        /// <summary>
        /// Adds an assertion that will be run after the request has completed, resolving a service from DI.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture ShouldHaveServiceWhich(Func<IServiceProvider, Task> assertion)
        {
            _assertions.Add(new ServiceAssertion
            {
                Assertion = assertion
            });

            return this;
        }

        /// <summary>
        /// Adds an assertion that a further HTTP request will be satisfied by the specified configured fixture.
        /// </summary>
        /// <param name="contextFactory">The context function.</param>
        /// <param name="configurator">The fixture configurator.</param>
        /// <returns></returns>
        public IMvcFunctionalTestFixture ShouldSatisfyRequest(Func<HttpResponseMessage, Task<object>> contextFactory,
            Action<object, IMvcFunctionalTestFixture> configurator)
        {
            _assertions.Add(new RequestAssertion
            {
                ContextFactory = contextFactory,
                Configurator = configurator
            });

            return this;
        }

        /// <summary>
        /// Builds this test fixture.
        /// </summary>
        /// <returns></returns>
        public IMvcFunctionalTestFixture Build()
        {
            // Building a client builds the server.
            using (Factory.CreateClient()) {}
            return this;
        }

        /// <summary>
        /// Gives a fixture an opportunity to configure the application before it gets built.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> for the application.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _built = true;

            var logger = _loggerProvider.CreateLogger(typeof(MvcFunctionalTestFixture<TStartup>).Name);

            logger.LogInformation("Bootstrapping test host");

            builder
                .ConfigureAppConfiguration(
                    (ctx, b) =>
                    {
                        foreach (var action in _configurationBuilders)
                        {
                            action(b);
                        }
                    })
                .UseEnvironment(_environment ?? EnvironmentName.Production)
                .ConfigureLogging(b =>
                {
                    b.AddProvider(_loggerProvider);

                    foreach (var configurator in _loggerConfigurators)
                    {
                        configurator(b);
                    }
                })
                .ConfigureServices(services =>
                {
                    foreach (var service in _extraServices)
                    {
                        services.Add(service);
                    }
                });
        }
        
        /// <summary>
        /// Runs this fixture.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <returns></returns>
        public async Task RunAsync(ITestOutputHelper output = null)
        {
            _run = true;

            try
            {
                if (RequestMessage == null)
                {
                    throw new InvalidOperationException($"Must call {nameof(When)} to configure an HTTP request");
                }

                if (!_assertions.Any())
                {
                    throw new InvalidOperationException("No assertions to run");
                }

                if (output != null)
                {
                    _loggerProvider.SetTestOutputHelper(output);
                }
                
                using var client = Factory.CreateClient();
                var provider = Factory.Server.Services;

                // Bootstrap.
                if (_bootstrapFunctions.Any())
                {
                    using var scope = provider.CreateScope();
                    foreach (var bootstrap in _bootstrapFunctions)
                    {
                        await bootstrap(scope.ServiceProvider);
                    }
                }

                var logger = provider.GetRequiredService<ILogger<MvcFunctionalTestFixture<TStartup>>>();

                var requestBody = await (RequestMessage.Content?.ReadAsStringAsync() ?? Task.FromResult<string>(null));
                logger.LogInformation($"Sending request {RequestMessage}\n{requestBody ?? "<no body>"}");

                var response = await client.SendAsync(RequestMessage);
                var responseBody = await (response.Content?.ReadAsStringAsync() ?? Task.FromResult<string>(null));
                logger.LogInformation($"Received {response}\n{responseBody ?? "<no body>"}");

                var aggregator = new ExceptionAggregator();

                foreach (var assertion in _assertions)
                {
                    switch (assertion)
                    {
                        case ResponseAssertion ra:
                            aggregator.Run(() => ra.Assertion(response));
                            break;

                        case ResponseBodyAssertion ra:
                            if (aggregator.TryRun(() => ra.Deserializer(responseBody), out var result))
                            {
                                foreach (var a in ra.Assertions)
                                {
                                    await aggregator.RunAsync(() => a(provider, result));
                                }
                            }
                            break;

                        case ServiceAssertion sa:
                            using (var scope = provider.CreateScope())
                            {
                                await aggregator.RunAsync(() => sa.Assertion(scope.ServiceProvider));
                            }
                            break;

                        case RequestAssertion ra:
                            var context = await ra.ContextFactory(response);
                            var fixture = new MvcFunctionalTestFixture<TStartup>(_loggerProvider, this);
                            ra.Configurator(context, fixture);
                            await aggregator.RunAsync(() => fixture.RunAsync());
                            break;
                    }
                }

                aggregator.ThrowIfHasExceptions();
            }
            finally
            {
                Reset();
            }
        }

        /// <summary>
        /// Gets the content root of the startup class.
        /// </summary>
        /// <returns></returns>
        public string GetContentRoot()
        {
            var metadataAttributes = GetContentRootMetadataAttributes();

            foreach (var contentRootAttribute in metadataAttributes)
            {
                var contentRootCandidate = Path.Combine(AppContext.BaseDirectory,
                    contentRootAttribute.ContentRootPath);

                var contentRootMarker = Path.Combine(contentRootCandidate,
                    Path.GetFileName(contentRootAttribute.ContentRootTest));

                if (File.Exists(contentRootMarker))
                {
                    return contentRootCandidate;
                }
            }

            throw new InvalidOperationException("Must properly reference xunit-fixture-mvc");
        }

        protected override void Dispose(bool disposing)
        {
            if (!_run && (RequestMessage != null || _assertions.Any()))
            {
                throw new InvalidOperationException("This fixture has not been run");
            }

            base.Dispose(disposing);
        }

        private void Reset()
        {
            RequestMessage = null;
            _assertions.Clear();
            _bootstrapFunctions.Clear();
        }

        private WebApplicationFactoryContentRootAttribute[] GetContentRootMetadataAttributes()
        {
            var tEntryPointAssemblyFullName = typeof(TStartup).Assembly.FullName;
            var tEntryPointAssemblyName = typeof(TStartup).Assembly.GetName().Name;

            var testAssembly = GetTestAssemblies();
            return testAssembly.SelectMany(a => a.GetCustomAttributes<WebApplicationFactoryContentRootAttribute>())
                .Where(a => string.Equals(a.Key, tEntryPointAssemblyFullName, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(a.Key, tEntryPointAssemblyName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Priority)
                .ToArray();
        }

        private IMvcFunctionalTestFixture WithSetup(Action action)
        {
            if (_built)
            {
                throw new InvalidOperationException("Cannot configure a built test fixture");
            }

            action();
            return this;
        }

        private abstract class Assertion
        {
        }

        private class ResponseAssertion : Assertion
        {
            public Action<HttpResponseMessage> Assertion { get; set; }
        }

        private class ResponseBodyAssertion : Assertion
        {
            public Func<string, object> Deserializer { get; set; }

            public ICollection<Func<IServiceProvider, object, Task>> Assertions { get; set; }
        }

        private class ServiceAssertion : Assertion
        {
            public Func<IServiceProvider, Task> Assertion { get; set; }
        }

        private class RequestAssertion : Assertion
        {
            public Func<HttpResponseMessage, Task<object>> ContextFactory { get; set; }

            public Action<object, IMvcFunctionalTestFixture> Configurator { get; set;  }
        }

    }
}