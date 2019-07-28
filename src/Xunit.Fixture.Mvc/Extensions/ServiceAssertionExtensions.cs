using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Xunit.Fixture.Mvc.Extensions
{
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
    }
}
