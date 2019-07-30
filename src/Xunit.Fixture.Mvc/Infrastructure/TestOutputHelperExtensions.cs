using System;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace Xunit.Fixture.Mvc.Infrastructure
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
        public static string GetCurrentTestName(this ITestOutputHelper output)
        {
            var testMember = output.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                 .FirstOrDefault(x => x.FieldType == typeof(ITest))
                             ?? throw new ArgumentException("Cannot find the test field");
            
            var test = (ITest) testMember.GetValue(output);
            return test.DisplayName;
        }
    }
}
