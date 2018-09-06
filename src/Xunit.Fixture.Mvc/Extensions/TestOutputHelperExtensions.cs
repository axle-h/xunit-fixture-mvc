using System;
using System.Reflection;
using Xunit.Abstractions;

namespace Xunit.Fixture.Mvc.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ITestOutputHelper"/>.
    /// </summary>
    public static class TestOutputHelperExtensions
    {
        /// <summary>
        /// Gets the name of the current test.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Cannot find the test field</exception>
        public static string GetCurrentTestName(this ITestOutputHelper output)
        {
            var testMember = output.GetType().GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            if (testMember == null)
            {
                throw new ArgumentException("Cannot find the test field");
            }
            var test = (ITest) testMember.GetValue(output);
            return test.DisplayName;
        }
    }
}
