using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xunit.Fixture.Mvc.Infrastructure
{
    /// <summary>
    /// Logger provider for logging to xunit test output.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Logging.ILoggerProvider" />
    public class TestOutputHelperLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _helper;
        private readonly Stopwatch _stopwatch;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TestOutputHelperLoggerProvider"/> class.
        /// </summary>
        /// <param name="helper">The helper.</param>
        public TestOutputHelperLoggerProvider(ITestOutputHelper helper)
        {
            _helper = helper;
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName) => new TestOutputHelperLogger(_helper, categoryName, _stopwatch);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        private class TestOutputHelperLogger : ILogger
        {
            private readonly ITestOutputHelper _helper;
            private readonly string _categoryName;
            private readonly Stopwatch _stopwatch;

            public TestOutputHelperLogger(ITestOutputHelper helper, string categoryName, Stopwatch stopwatch)
            {
                _helper = helper;
                _categoryName = categoryName;
                _stopwatch = stopwatch;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = $"{_stopwatch.Elapsed} [{logLevel}] [{_categoryName}] {formatter(state, exception)} {exception}";

                try
                {
                    _helper.WriteLine(message);
                }
                catch (Exception)
                {
                    // When called from a background thread, the xunit test output will fail.
                    // So let's fall back to just writing to the console and debug streams.
                    Console.WriteLine(message);
                    Debug.WriteLine(message);
                }
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public IDisposable BeginScope<TState>(TState state) => new Scope();

            private class Scope : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}