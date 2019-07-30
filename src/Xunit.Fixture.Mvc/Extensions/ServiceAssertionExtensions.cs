using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class ServiceAssertionExtensions
    {
        /// <summary>
        /// Adds an assertion that will be run after the request has completed, resolving a service from DI.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldHaveProvidedServiceWhich<TService>(this IMvcFunctionalTestFixture fixture,
            Func<TService, Task> assertion)
            where TService : class =>
            fixture.ShouldHaveServiceWhich(p => assertion(p.GetRequiredService<TService>()));

        /// <summary>
        /// Adds an assertion that will trigger another HTTP request which has access to the deserialized response body of the main fixture request.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldSatisfyJsonRequest<TResponse>(
            this IMvcFunctionalTestFixture fixture,
            Action<TResponse, IMvcFunctionalTestFixture> configurator) =>
            fixture.ShouldSatisfyRequest(async m =>
                {
                    var responseBody = await (m.Content?.ReadAsStringAsync() ?? Task.FromResult<string>(null));
                    if (responseBody == null)
                    {
                        return default;
                    }
                    return JsonConvert.DeserializeObject<TResponse>(responseBody);
                },
                (o, f) => configurator((TResponse) o, f));

        /// <summary>
        /// Adds an assertion that will trigger another HTTP request which has access to the deserialized response body of the main fixture request.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldSatisfyRequest(
            this IMvcFunctionalTestFixture fixture,
            Action<IMvcFunctionalTestFixture> configurator) =>
            fixture.ShouldSatisfyRequest(m => Task.FromResult(null as object),
                (o, f) => configurator(f));
    }
}
