using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class MvcFunctionalTestFixtureConvenienceExtensions
    {
        /// <summary>
        /// Creates an auto fixture constructed instance of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TModel Create<TModel>(this IMvcFunctionalTestFixture fixture) => fixture.AutoFixture.Create<TModel>();

        /// <summary>
        /// Creates a collection of auto fixture constructed instances of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static ICollection<TModel> CreateMany<TModel>(this IMvcFunctionalTestFixture fixture) => fixture.AutoFixture.CreateMany<TModel>().ToList();

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
            fixture.HavingConfiguredHttpMessage(message => message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token));

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
    }
}
