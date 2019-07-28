using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for configuring an <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class RestActExtensions
    {
        private static readonly HttpMethod Patch = new HttpMethod("PATCH");

        /// <summary>
        /// Configures the fixture perform the specified HTTP action.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The HTTP content.</param>
        public static IMvcFunctionalTestFixture When(this IMvcFunctionalTestFixture fixture, HttpMethod method, string uri, HttpContent content) =>
            fixture.When(message =>
            {
                message.Method = method;
                message.RequestUri = new Uri(uri, UriKind.Relative);
                message.Content = content;
            });

        /// <summary>
        /// Configures the specified fixture's act step to be an HTTP request with the specified method, url and body.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenCallingRestMethod(this IMvcFunctionalTestFixture fixture, HttpMethod method, string url, object body = null)
        {
            var content = body == null ? null : new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            return fixture.When(method, url, content);
        }

        /// <summary>
        /// Configures the specified fixture's act step to be an HTTP request with the specified method, url and body.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenCallingRestMethod<TModel>(this IMvcFunctionalTestFixture fixture,
                                                                              HttpMethod method,
                                                                              string url,
                                                                              out TModel model) =>
            fixture.HavingModel(out model)
                   .WhenCallingRestMethod(method, url, model);

        /// <summary>
        /// Configures the specified fixture's act step to be a GET request at the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenGetting(this IMvcFunctionalTestFixture fixture, string url) =>
            fixture.WhenCallingRestMethod(HttpMethod.Get, url);

        /// <summary>
        /// Configures the specified fixture's act step to be a GET request for the specified entity and id.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenGettingById<TId>(this IMvcFunctionalTestFixture fixture, string entity, TId id) =>
            fixture.WhenCallingRestMethod(HttpMethod.Get, $"{entity}/{Uri.EscapeDataString(id.ToString())}");

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenUpdating<TId, TBody>(this IMvcFunctionalTestFixture fixture, string entity, TId id, TBody body) =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, $"{entity}/{Uri.EscapeDataString(id.ToString())}", body);

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenUpdating<TId, TModel>(this IMvcFunctionalTestFixture fixture,
                                                                          string entity,
                                                                          TId id,
                                                                          out TModel model) =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, $"{entity}/{Uri.EscapeDataString(id.ToString())}", out model);

        /// <summary>
        /// Configures the specified fixture's act step to be a POST request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenCreating<TBody>(this IMvcFunctionalTestFixture fixture, string entity, TBody body) =>
            fixture.WhenCallingRestMethod(HttpMethod.Post, entity, body);


        /// <summary>
        /// Configures the specified fixture's act step to be a POST request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenCreating<TModel>(this IMvcFunctionalTestFixture fixture, string entity, out TModel model) =>
            fixture.WhenCallingRestMethod(HttpMethod.Post, entity, out model);

        /// <summary>
        /// Configures the specified fixture's act step to be a PATCH request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenPatching<TId, TBody>(this IMvcFunctionalTestFixture fixture, string entity, TId id, TBody body) =>
            fixture.WhenCallingRestMethod(Patch, $"{entity}/{Uri.EscapeDataString(id.ToString())}", body);

        /// <summary>
        /// Configures the specified fixture's act step to be a PATCH request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenPatching<TId, TModel>(this IMvcFunctionalTestFixture fixture,
                                                                          string entity,
                                                                          TId id,
                                                                          out TModel model) =>
            fixture.WhenCallingRestMethod(Patch, $"{entity}/{Uri.EscapeDataString(id.ToString())}", out model);

        /// <summary>
        /// Configures the specified fixture's act step to be a DELETE request for the specified entity and id.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenDeleting<TId>(this IMvcFunctionalTestFixture fixture, string entity, TId id) =>
            fixture.WhenCallingRestMethod(HttpMethod.Delete, $"{entity}/{Uri.EscapeDataString(id.ToString())}");
    }
}