using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class ConvenienceExtensions
    {
        /// <summary>
        /// Performs an arrange action.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture Having(this IMvcFunctionalTestFixture fixture, Action action)
        {
            action();
            return fixture;
        }

        /// <summary>
        /// Performs an arrange function, returning the result as an output parameter.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture Having<TResult>(this IMvcFunctionalTestFixture fixture, Func<TResult> action, out TResult result)
        {
            result = action();
            return fixture;
        }

        /// <summary>
        /// Picks a random model from the specified collection and optionally mutates it.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="model">The model.</param>
        /// <param name="mutationFunc">The mutation function.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingRandom<TModel>(this IMvcFunctionalTestFixture fixture,
                                                                     ICollection<TModel> models,
                                                                     out TModel model,
                                                                     Action<TModel> mutationFunc = null)
        {
            model = fixture.Faker.Random.CollectionItem(models);
            mutationFunc?.Invoke(model);
            return fixture;
        }

        /// <summary>
        /// Creates an auto fixture constructed instance of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingModel<TModel>(this IMvcFunctionalTestFixture fixture,
                                                                    out TModel model,
                                                                    Action<Faker, TModel> configurator = null)
        {
            model = fixture.AutoFixture.Create<TModel>();
            configurator?.Invoke(fixture.Faker, model);
            return fixture;
        }

        /// <summary>
        /// Creates a collection of auto fixture constructed instances of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingModels<TModel>(this IMvcFunctionalTestFixture fixture,
                                                                     out ICollection<TModel> models,
                                                                     Action<Faker, TModel> configurator = null)
        {
            models = fixture.AutoFixture.CreateMany<TModel>().ToList();

            if (configurator != null)
            {
                foreach (var model in models)
                {
                    configurator(fixture.Faker, model);
                }
            }

            return fixture;
        }

        /// <summary>
        /// Uses the faker on the fixture as a factory for some fake data.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="fake">The fake.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingFake<TFake>(this IMvcFunctionalTestFixture fixture,
                                                                  Func<Faker, TFake> factory,
                                                                  out TFake fake)
        {
            fake = factory(fixture.Faker);
            return fixture;
        }

        /// <summary>
        /// Configures the HTTP client to have the specified path base.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingPathBase(this IMvcFunctionalTestFixture fixture, string path) =>
            fixture.HavingClientConfiguration(o => o.BaseAddress = new UriBuilder("http://localhost")
            {
                Path = path.TrimStart('/').TrimEnd('/') + '/'
            }.Uri);

        /// <summary>
        /// Configures the request to use the specified string as a bearer token in the authorization header.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingBearerToken(this IMvcFunctionalTestFixture fixture, string token) =>
            fixture.When(message => message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingAutoFixtureCustomization<TModel>(this IMvcFunctionalTestFixture fixture,
                                                                                       Func<ICustomizationComposer<TModel>, ISpecimenBuilder> composer)
        {
            fixture.AutoFixture.Customize(composer);
            return fixture;
        }

        /// <summary>
        /// Sets the log minimum level.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingMinimumLogLevel(this IMvcFunctionalTestFixture fixture, LogLevel logLevel) =>
            fixture.HavingLogging(x => x.SetMinimumLevel(logLevel));

        /// <summary>
        /// Adds a bootstrap action to the test fixture that will be run before the request.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="bootstrapFunction">The bootstrap function.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingBootstrap<TService>(this IMvcFunctionalTestFixture fixture,
            Func<TService, Task> bootstrapFunction) =>
            fixture.HavingBootstrap(p => bootstrapFunction(p.GetRequiredService<TService>()));
    }
}
