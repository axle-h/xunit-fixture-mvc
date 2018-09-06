using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xunit.Fixture.Mvc.Infrastructure
{
    /// <summary>
    /// Utility class for collecting aggregations before on dispose:
    /// 1. If no exceptions were collected, do nothing.
    /// 2. If a single exception was collected, throw it.
    /// 3. If multiple exceptions were collection, throw an <see cref="AggregateException"/>.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class ExceptionAggregator : IDisposable
    {
        private readonly ICollection<Exception> _exceptions = new List<Exception>();

        /// <summary>
        /// Tries the specified action, adding any thrown exceptions to the aggregator.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }

        /// <summary>
        /// Tries the specified action, adding any thrown exceptions to the aggregator.
        /// </summary>
        /// <param name="action">The action.</param>
        public async Task TryAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }

        /// <summary>
        /// Adds the specified exception to the aggregator.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Add(Exception exception) => _exceptions.Add(exception);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="AggregateException"></exception>
        public void Dispose()
        {
            if (!_exceptions.Any())
            {
                return;
            }

            if (_exceptions.Count == 1)
            {
                throw _exceptions.First();
            }

            throw new AggregateException(_exceptions);
        }
    }
}