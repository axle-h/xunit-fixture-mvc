using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Newtonsoft.Json;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class ResponseAssertionExtensions
    {
        /// <summary>
        /// Adds assertions that will be run on the HTTP, JSON response body.
        /// </summary>
        /// <typeparam name="TModel">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnJson<TModel>(this IMvcFunctionalTestFixture fixture, params Action<TModel>[] assertions) =>
            fixture.ShouldReturnBody(JsonConvert.DeserializeObject<TModel>, assertions);

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="options">The equivalency assertion options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnEquivalentJson<TResponseModel, TModel>(
            this IMvcFunctionalTestFixture fixture,
            TModel model,
            Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options = null) =>
            fixture.ShouldReturnJson<TResponseModel>(x => x.Should().BeEquivalentTo(model, options ?? (c => c)));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="options">The equivalency assertion options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnEquivalentJson<TModel>(
            this IMvcFunctionalTestFixture fixture,
            TModel model,
            Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options = null) =>
            fixture.ShouldReturnEquivalentJson<TModel, TModel>(model, options);

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result returned will be an empty collection.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnEmptyJsonCollection(this IMvcFunctionalTestFixture fixture) =>
            fixture.ShouldReturnJson<ICollection<object>>(x => x.Should().BeEmpty());

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result returned will be a collection of the specified length.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnJsonCollectionOfLength(this IMvcFunctionalTestFixture fixture, int count) =>
            fixture.ShouldReturnJson<ICollection<object>>(x => x.Should().HaveCount(count));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnJsonCollectionContainingEquivalent<TResponseModel, TModel>(
            this IMvcFunctionalTestFixture fixture,
            TModel model,
            Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options = null) =>
            fixture.ShouldReturnJson<ICollection<TResponseModel>>(x => x.Should().ContainEquivalentOf(model, options ?? (c => c)));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnJsonCollectionContainingEquivalent<TResponseModel, TModel>(
            this IMvcFunctionalTestFixture fixture,
            ICollection<TModel> models,
            Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options = null) =>
            fixture.ShouldReturnJson(models.Select<TModel, Action<ICollection<TResponseModel>>>(e =>
                    x => x.Should().ContainEquivalentOf(e, options ?? (c => c)))
                .ToArray());

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnJsonCollectionContainingEquivalent<TModel>(this IMvcFunctionalTestFixture fixture,
            TModel model,
            Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options = null) =>
            fixture.ShouldReturnJsonCollectionContainingEquivalent<TModel, TModel>(model, options);

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnJsonCollectionContainingEquivalent<TModel>(this IMvcFunctionalTestFixture fixture,
            ICollection<TModel> models,
            Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options = null) =>
            fixture.ShouldReturnJsonCollectionContainingEquivalent<TModel, TModel>(models, options);
        
        /// <summary>
        /// Adds an assert step that the HTTP response had a success status code i.e. 2xx.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnSuccessfulStatus(this IMvcFunctionalTestFixture fixture) =>
            fixture.ShouldReturn(r => r.EnsureSuccessStatusCode());

        /// <summary>
        /// Adds an assert step that the HTTP response was a permanent redirect (301) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnPermanentRedirect(this IMvcFunctionalTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.Moved, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a redirect (302) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnRedirect(this IMvcFunctionalTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.Redirect, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a see other (303 - redirect to a GET) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnSeeOther(this IMvcFunctionalTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.SeeOther, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a temporary redirect (307) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnTemporaryRedirect(this IMvcFunctionalTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.TemporaryRedirect, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response had a bad request (400) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnBadRequest(this IMvcFunctionalTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.BadRequest);

        /// <summary>
        /// Adds an assert step that the HTTP response had an unauthorized (401) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnUnauthorized(this IMvcFunctionalTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.Unauthorized);

        /// <summary>
        /// Adds an assert step that the HTTP response had a forbidden (403) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnForbidden(this IMvcFunctionalTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.Forbidden);

        /// <summary>
        /// Adds an assert step that the HTTP response had a not found (404) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnNotFound(this IMvcFunctionalTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.NotFound);

        /// <summary>
        /// Adds an assert step that the HTTP response had a internal server error (500) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnInternalServerError(this IMvcFunctionalTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.InternalServerError);

        private static IMvcFunctionalTestFixture ShouldReturnStatus(this IMvcFunctionalTestFixture fixture, HttpStatusCode statusCode) =>
            fixture.ShouldReturn(r => r.StatusCode.Should().Be(statusCode));

        private static IMvcFunctionalTestFixture ShouldReturnRedirect(this IMvcFunctionalTestFixture fixture, HttpStatusCode statusCode, string redirectUrl) =>
            fixture.ShouldReturn(r => r.StatusCode.Should().Be(statusCode),
                                   r => r.Headers.Location.Should().Be(redirectUrl));
    }
}