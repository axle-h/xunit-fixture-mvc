using System.Collections.Generic;
using System.Net;
using FluentAssertions;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class MvcFunctionalTestFixtureAssertionExtensions
    {
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