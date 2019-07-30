using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Fixture.Mvc.Infrastructure
{
    /// <summary>
    /// Logger provider for logging to xunit test output.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Logging.ILoggerProvider" />
    public class TestOutputHelperLoggerProvider : ILoggerProvider
    {
        private readonly Stopwatch _stopwatch;
        private readonly IMessageSink _sink;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TestOutputHelperLoggerProvider"/> class.
        /// </summary>
        /// <param name="sink">The message sink.</param>
        /// <param name="testOutput">The test output helper.</param>
        public TestOutputHelperLoggerProvider(IMessageSink sink, ITestOutputHelper testOutput)
        {
            _sink = sink;
            TestOutput = testOutput;
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// The test output helper.
        /// </summary>
        public ITestOutputHelper TestOutput { get; private set; }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName) =>
            new TestOutputHelperLogger(_sink, TestOutput, categoryName, _stopwatch);
        
        /// <summary>
        /// Sets the test output helper for the next test.
        /// </summary>
        /// <param name="output">The test output helper</param>
        public void SetTestOutputHelper(ITestOutputHelper output)
        {
            TestOutput = output;
            _stopwatch.Restart();
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        private class TestOutputHelperLogger : ILogger
        {
            private readonly IMessageSink _sink;
            private readonly ITestOutputHelper _output;
            private readonly string _categoryName;
            private readonly Stopwatch _stopwatch;

            public TestOutputHelperLogger(IMessageSink sink, ITestOutputHelper output, string categoryName, Stopwatch stopwatch)
            {
                _sink = sink;
                _output = output;
                _categoryName = categoryName;
                _stopwatch = stopwatch;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = $"{_stopwatch.Elapsed} [{logLevel}] [{_categoryName}] {formatter(state, exception)} {exception}";

                void WriteMessageSink()
                {
                    if (_sink != null)
                    {
                        // First try the message sink.
                        _sink.OnMessage(new DiagnosticMessage(message));
                    }
                    else
                    {
                        // Fall back to just writing to the console and debug streams.
                        Console.WriteLine(message);
                        Debug.WriteLine(message);
                    }
                }

                if (_output != null)
                {
                    try
                    {
                        _output.WriteLine(message);
                    }
                    catch (Exception)
                    {
                        // When called from a background thread, the xunit test output will fail.
                        WriteMessageSink();
                    }
                }
                else
                {
                    WriteMessageSink();
                }
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public IDisposable BeginScope<TState>(TState state) => new DummyScope();
        }

        private class DummyScope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}