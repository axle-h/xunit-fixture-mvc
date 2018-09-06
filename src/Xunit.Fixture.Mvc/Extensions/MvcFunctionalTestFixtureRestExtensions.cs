using System;
using System.Net;
using System.Net.Http;
using System.Text;
using AutoFixture;
using FluentAssertions;
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
        /// Configures the specified fixture's act step to be a GET request for the specified entity.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static TFixture WhenGettingAll<TFixture>(this TFixture fixture, string entity)
            where TFixture : IMvcFunctionalTestFixture =>
            fixture.WhenCallingRestMethod(HttpMethod.Get, entity);

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

        /// <summary>
        /// Adds an assert step that the HTTP response had a success status code i.e. 2xx.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnSuccessfulStatus<TFixture>(this TFixture fixture)
            where TFixture : IMvcFunctionalTestFixture
        {
            fixture.ResponseShould(r => r.EnsureSuccessStatusCode());
            return fixture;
        }

        /// <summary>
        /// Adds an assert step that the HTTP response was a permanent redirect (301) to the specified url.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnPermanentRedirect<TFixture>(this TFixture fixture, string redirectUrl)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnRedirect(HttpStatusCode.Moved, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a redirect (302) to the specified url.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnRedirect<TFixture>(this TFixture fixture, string redirectUrl)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnRedirect(HttpStatusCode.Redirect, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a see other (303 - redirect to a GET) to the specified url.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnSeeOther<TFixture>(this TFixture fixture, string redirectUrl)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnRedirect(HttpStatusCode.SeeOther, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a temporary redirect (307) to the specified url.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnTemporaryRedirect<TFixture>(this TFixture fixture, string redirectUrl)
            where TFixture : IMvcFunctionalTestFixture 
            => fixture.ShouldReturnRedirect(HttpStatusCode.TemporaryRedirect, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response had a bad request (400) status code.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnBadRequest<TFixture>(this TFixture fixture)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnStatus(HttpStatusCode.BadRequest);

        /// <summary>
        /// Adds an assert step that the HTTP response had an unauthorized (401) status code.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnUnauthorized<TFixture>(this TFixture fixture)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnStatus(HttpStatusCode.Unauthorized);

        /// <summary>
        /// Adds an assert step that the HTTP response had a forbidden (403) status code.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnForbidden<TFixture>(this TFixture fixture)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnStatus(HttpStatusCode.Forbidden);

        /// <summary>
        /// Adds an assert step that the HTTP response had a not found (404) status code.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnNotFound<TFixture>(this TFixture fixture)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnStatus(HttpStatusCode.NotFound);

        /// <summary>
        /// Adds an assert step that the HTTP response had a internal server error (500) status code.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnInternalServerError<TFixture>(this TFixture fixture)
            where TFixture : IMvcFunctionalTestFixture
            => fixture.ShouldReturnStatus(HttpStatusCode.InternalServerError);

        private static TFixture ShouldReturnStatus<TFixture>(this TFixture fixture, HttpStatusCode statusCode)
            where TFixture : IMvcFunctionalTestFixture
        {
            fixture.ResponseShould(r => r.StatusCode.Should().Be(statusCode));
            return fixture;
        }

        private static TFixture ShouldReturnRedirect<TFixture>(this TFixture fixture, HttpStatusCode statusCode, string redirectUrl)
            where TFixture : IMvcFunctionalTestFixture
        {
            fixture.ResponseShould(r => r.StatusCode.Should().Be(statusCode),
                                   r => r.Headers.Location.Should().Be(redirectUrl));
            return fixture;
        }
    }
}