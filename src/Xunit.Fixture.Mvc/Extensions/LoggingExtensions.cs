using Microsoft.Extensions.Logging;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMvcFunctionalTestFixture"/> to help with logging.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture Log(this IMvcFunctionalTestFixture fixture, LogLevel logLevel,
            string message, params object[] args)
        {
            fixture.Logger.Log(logLevel, message, args);
            return fixture;
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture LogTrace(this IMvcFunctionalTestFixture fixture,
            string message, params object[] args) => fixture.Log(LogLevel.Trace, message, args);

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture LogDebug(this IMvcFunctionalTestFixture fixture,
            string message, params object[] args) => fixture.Log(LogLevel.Debug, message, args);

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture LogInformation(this IMvcFunctionalTestFixture fixture,
            string message, params object[] args) => fixture.Log(LogLevel.Information, message, args);

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture LogWarning(this IMvcFunctionalTestFixture fixture,
            string message, params object[] args) => fixture.Log(LogLevel.Warning, message, args);

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture LogError(this IMvcFunctionalTestFixture fixture,
            string message, params object[] args) => fixture.Log(LogLevel.Error, message, args);

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IMvcFunctionalTestFixture LogCritical(this IMvcFunctionalTestFixture fixture,
            string message, params object[] args) => fixture.Log(LogLevel.Critical, message, args);
    }
}
