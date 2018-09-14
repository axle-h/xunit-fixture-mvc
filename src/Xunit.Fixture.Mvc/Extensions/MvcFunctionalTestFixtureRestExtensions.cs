using System;
using System.Net.Http;
using System.Text;
using AutoFixture;
using Newtonsoft.Json;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for configuring an <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class MvcFunctionalTestFixtureRestExtensions
    {
        private static readonly HttpMethod Patch = new HttpMethod("PATCH");

        /// <summary>
        /// Configures the specified fixture's act step to be a GET request at the specified url.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static TFixture WhenGetting<TFixture>(this TFixture fixture, string url)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Get, url);
        
        /// <summary>
        /// Configures the specified fixture's act step to be a GET request for the specified entity and id.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static TFixture WhenGettingById<TFixture, TId>(this TFixture fixture, string entity, TId id)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Get, $"{entity}/{Uri.EscapeDataString(id.ToString())}");

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static TFixture WhenUpdating<TFixture, TId, TBody>(this TFixture fixture, string entity, TId id, TBody body)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, $"{entity}/{Uri.EscapeDataString(id.ToString())}", body);

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static TFixture WhenUpdating<TFixture, TId, TModel>(this TFixture fixture, string entity, TId id, out TModel model)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, $"{entity}/{Uri.EscapeDataString(id.ToString())}", out model);

        /// <summary>
        /// Configures the specified fixture's act step to be a POST request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static TFixture WhenCreating<TFixture, TBody>(this TFixture fixture, string entity, TBody body)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Post, entity, body);


        /// <summary>
        /// Configures the specified fixture's act step to be a POST request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static TFixture WhenCreating<TFixture, TModel>(this TFixture fixture, string entity, out TModel model)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Post, entity, out model);

        /// <summary>
        /// Configures the specified fixture's act step to be a PATCH request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static TFixture WhenPatching<TFixture, TId, TBody>(this TFixture fixture, string entity, TId id, TBody body)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(Patch, $"{entity}/{Uri.EscapeDataString(id.ToString())}", body);

        /// <summary>
        /// Configures the specified fixture's act step to be a PATCH request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static TFixture WhenPatching<TFixture, TId, TModel>(this TFixture fixture, string entity, TId id, out TModel model)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(Patch, $"{entity}/{Uri.EscapeDataString(id.ToString())}", out model);

        /// <summary>
        /// Configures the specified fixture's act step to be a DELETE request for the specified entity and id.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static TFixture WhenDeleting<TFixture, TId>(this TFixture fixture, string entity, TId id)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Delete, $"{entity}/{Uri.EscapeDataString(id.ToString())}");

        /// <summary>
        /// Configures the specified fixture's act step to be aN HTTP request with the specified method, url and body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static TFixture WhenCallingRestMethod<TFixture>(this TFixture fixture, HttpMethod method, string url, object body = null)
            where TFixture : IMvcFunctionalTestFixture
        {
            var content = body == null ? null : new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            fixture.When(method, url, content);
            return fixture;
        }

        /// <summary>
        /// Configures the specified fixture's act step to be aN HTTP request with the specified method, url and body.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static TFixture WhenCallingRestMethod<TFixture, TModel>(this TFixture fixture, HttpMethod method, string url, out TModel model)
            where TFixture : IMvcFunctionalTestFixture
        {
            model = fixture.AutoFixture.Create<TModel>();
            return fixture.WhenCallingRestMethod(method, url, model);
        }
    }
}