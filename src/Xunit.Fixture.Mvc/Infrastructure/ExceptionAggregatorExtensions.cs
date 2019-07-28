using System;
using Xunit.Sdk;

namespace Xunit.Fixture.Mvc.Infrastructure
{
    public static class ExceptionAggregatorExtensions
    {
        /// <summary>
        /// Check if the specified exception aggregator has exceptions and throw if it does.
        /// </summary>
        /// <param name="aggregator">The aggregator.</param>
        public static void ThrowIfHasExceptions(this ExceptionAggregator aggregator)
        {
            if (aggregator.HasExceptions)
            {
                throw aggregator.ToException();
            }
        }

        /// <summary>
        /// Attempts to run the specified function and get the result.
        /// Adds any thrown exceptions to the specified aggregator.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="aggregator">The aggregator.</param>
        /// <param name="function">The function.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryRun<TResult>(this ExceptionAggregator aggregator, Func<TResult> function, out TResult result)
        {
            try
            {
                result = function();
                return true;
            }
            catch (Exception e)
            {
                aggregator.Add(e);
            }

            result = default;
            return false;
        }
    }
}
