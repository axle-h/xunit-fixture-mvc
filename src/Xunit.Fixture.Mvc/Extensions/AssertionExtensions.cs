using System.Collections.Generic;
using System.Net;
using FluentAssertions;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnEquivalentResponse<TModel>(this IMvcFunctionalTestFixture fixture, TModel model) =>
            fixture.ShouldReturnEquivalentResponse<TModel, TModel>(model);

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnEquivalentResponse<TResponseModel, TModel>(this IMvcFunctionalTestFixture fixture, TModel model) =>
            fixture.ShouldReturnJson<TResponseModel>(x => x.Should().BeEquivalentTo(model));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result returned will be an empty collection.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnEmptyCollection(this IMvcFunctionalTestFixture fixture) =>
            fixture.ShouldReturnJson<ICollection<object>>(x => x.Should().BeEmpty());

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result returned will be a collection of the specified length.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnCollectionOfLength(this IMvcFunctionalTestFixture fixture, int count) =>
            fixture.ShouldReturnJson<ICollection<object>>(x => x.Should().HaveCount(count));

        /// <summary>
        /// Adds an assert step that the HTTP response had a success status code i.e. 2xx.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture ShouldReturnSuccessfulStatus(this IMvcFunctionalTestFixture fixture) =>
            fixture.ResponseShould(r => r.EnsureSuccessStatusCode());

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
            fixture.ResponseShould(r => r.StatusCode.Should().Be(statusCode));

        private static IMvcFunctionalTestFixture ShouldReturnRedirect(this IMvcFunctionalTestFixture fixture, HttpStatusCode statusCode, string redirectUrl) =>
            fixture.ResponseShould(r => r.StatusCode.Should().Be(statusCode),
                                   r => r.Headers.Location.Should().Be(redirectUrl));
    }
}