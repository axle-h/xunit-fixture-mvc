using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IPostprocessComposer<TModel> With<TModel, TProperty>(
            this IPostprocessComposer<TModel> composer,
            Expression<Func<TModel, TProperty>> property,
            Func<TProperty> factory)
        {
            var info = property.GetProperty();
            return composer.Without(property).Do(x => info.SetValue(x, factory()));
        }

        /// <summary>
        /// Configures AutoFixture.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingAutoFixture(
            this IMvcFunctionalTestFixture fixture,
            Action<IFixture> action)
        {
            action(fixture.AutoFixture);
            return fixture;
        }

        /// <summary>
        /// Adds the specified behavior to the auto fixture instance.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingAutoFixtureBehavior(
            this IMvcFunctionalTestFixture fixture,
            ISpecimenBuilderTransformation behavior) =>
            fixture.HavingAutoFixture(f => f.Behaviors.Add(behavior));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingAutoFixtureCustomization<TModel>(
            this IMvcFunctionalTestFixture fixture,
            Func<ICustomizationComposer<TModel>, ISpecimenBuilder> composer) =>
            fixture.HavingAutoFixture(f => f.Customize(composer));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingAutoFixtureCustomization<TModel>(
            this IMvcFunctionalTestFixture fixture,
            Func<Faker, ICustomizationComposer<TModel>, ISpecimenBuilder> composer) =>
            fixture.HavingAutoFixture(f => f.Customize<TModel>(c => composer(fixture.Faker, c)));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="specimenBuilder">The specimen builder.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingAutoFixtureCustomization(
            this IMvcFunctionalTestFixture fixture,
            ISpecimenBuilder specimenBuilder) =>
            fixture.HavingAutoFixture(f => f.Customizations.Add(specimenBuilder));

        /// <summary>
        /// Adds the omit on recursion behaviour to AutoFixture.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingRecursiveModelSupport(
            this IMvcFunctionalTestFixture fixture) =>
            fixture.HavingAutoFixtureBehavior(new OmitOnRecursionBehavior());

        /// <summary>
        /// Creates an auto fixture constructed instance of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingModel<TModel>(
            this IMvcFunctionalTestFixture fixture,
            out TModel model,
            Action<Faker, TModel> configurator = null)
        {
            model = fixture.AutoFixture.Create<TModel>();
            configurator?.Invoke(fixture.Faker, model);
            return fixture;
        }

        /// <summary>
        /// Creates a collection of auto fixture constructed instances of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingModels<TModel>(
            this IMvcFunctionalTestFixture fixture,
            out ICollection<TModel> models,
            Action<Faker, TModel> configurator = null)
        {
            models = fixture.AutoFixture.CreateMany<TModel>().ToList();

            if (configurator != null)
            {
                foreach (var model in models)
                {
                    configurator(fixture.Faker, model);
                }
            }

            return fixture;
        }
    }
}
