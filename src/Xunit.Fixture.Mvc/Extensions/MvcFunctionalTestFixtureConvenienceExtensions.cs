using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;

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

        public static IMvcFunctionalTestFixture HavingPathBase(this IMvcFunctionalTestFixture fixture, string path) =>
            fixture.HavingClientConfiguration(o => o.BaseAddress = new UriBuilder(o.BaseAddress)
                                                                   {
                                                                       Path = path.TrimStart('/').TrimEnd('/') + '/'
                                                                   }.Uri);
    }
}
