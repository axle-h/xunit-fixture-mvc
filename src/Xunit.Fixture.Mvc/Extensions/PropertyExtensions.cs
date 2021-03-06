﻿using System;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/>.
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// Adds the specified property value to the specified key.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingProperty(this IMvcFunctionalTestFixture fixture, string key, object value)
        {
            fixture.Properties[key] = value;
            return fixture;
        }

        /// <summary>
        /// Adds the property value generated by the specified factory to the specified key.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingProperty(this IMvcFunctionalTestFixture fixture, string key, Func<object> valueFactory) =>
            fixture.HavingProperty(key, valueFactory());

        /// <summary>
        /// Runs the specified fixture action if a property exists in the fixture that matches the specified value predicate.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="key">The key.</param>
        /// <param name="valuePredicate">The value predicate.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenHavingStringProperty(this IMvcFunctionalTestFixture fixture,
                                                                         string key,
                                                                         Func<string, bool> valuePredicate,
                                                                         Action<IMvcFunctionalTestFixture> action)
        {
            if (fixture.Properties.TryGetValue(key, out var value) && value is string s && valuePredicate(s))
            {
                action(fixture);
            }

            return fixture;
        }

        /// <summary>
        /// Runs the specified fixture action if a property exists in the fixture that matches the specified value.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture WhenHavingStringProperty(this IMvcFunctionalTestFixture fixture,
                                                                         string key,
                                                                         string value,
                                                                         Action<IMvcFunctionalTestFixture> action) =>
            fixture.WhenHavingStringProperty(key, x => x == value, action);

        /// <summary>
        /// Gets the property of the specified type with the specified key.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static TProperty GetProperty<TProperty>(this IMvcFunctionalTestFixture fixture, string key)
        {
            if (!fixture.Properties.ContainsKey(key))
            {
                throw new ArgumentException("Cannot find test fixture property with key: " + key);
            }

            if (fixture.Properties[key] is TProperty p)
            {
                return p;
            }

            throw new ArgumentException($"Test fixture property with key: {key} is not a {typeof(TProperty)}");
        }

        /// <summary>
        /// Gets the property of the specified type with the specified key.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="key">The key.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingConfiguredProperty<TProperty>(
            this IMvcFunctionalTestFixture fixture,
            string key,
            out TProperty property)
        {
            property = fixture.GetProperty<TProperty>(key);
            return fixture;
        }

        /// <summary>
        /// Gets the property of the specified type with the specified key and runs the specified property action.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="key">The key.</param>
        /// <param name="propertyAction">The property action.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture HavingConfiguredProperty<TProperty>(
            this IMvcFunctionalTestFixture fixture,
            string key,
            Action<TProperty> propertyAction) =>
            fixture
                .HavingConfiguredProperty<TProperty>(key, out var property)
                .Having(() => propertyAction(property));
    }
}