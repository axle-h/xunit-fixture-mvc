using System;
using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Bogus;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for AutoFixture.
    /// </summary>
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Registers that a writable property should be assigned to the result of calling
        /// the specified factory function as part of specimen post-processing.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="composer">The composer.</param>
        /// <param name="property">The property.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static IPostprocessComposer<TModel> With<TModel, TProperty>(this IPostprocessComposer<TModel> composer,
                                                                           Expression<Func<TModel, TProperty>> property,
                                                                           Func<Faker, TProperty> factory)
        {
            var faker = new Faker();
            var info = property.GetProperty();
            return composer.Without(property).Do(x => info.SetValue(x, factory(faker)));
        }

        /// <summary>
        /// Registers that a writable string property and normalized equivalent should be assigned to the result of calling
        /// the specified factory function as part of specimen post-processing.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="composer">The composer.</param>
        /// <param name="property">The property.</param>
        /// <param name="normalizedProperty">The normalized property.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static IPostprocessComposer<TModel> WithNormalized<TModel>(this IPostprocessComposer<TModel> composer,
                                                                          Expression<Func<TModel, string>> property,
                                                                          Expression<Func<TModel, string>> normalizedProperty,
                                                                          Func<Faker, string> factory)
        {
            var faker = new Faker();
            var info = property.GetProperty();
            var normalizedInfo = normalizedProperty.GetProperty();

            return composer.Without(property).Without(normalizedProperty).Do(x =>
            {
                var value = factory(faker);
                info.SetValue(x, value);
                normalizedInfo.SetValue(x, value.Normalize().ToUpper());
            });
        }

        /// <summary>
        /// Adds the specified behavior to the auto fixture instance.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingAutoFixtureBehavior(this IMvcFunctionalTestFixture fixture, ISpecimenBuilderTransformation behavior)
        {
            fixture.AutoFixture.Behaviors.Add(behavior);
            return fixture;
        }
    }
}
