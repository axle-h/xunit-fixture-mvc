using Xunit.Abstractions;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// A functional test fixture for MVC.
    /// This only exists to proxy the message sink constructor for the xunit class fixture.
    /// It will throw unless it finds a parameterless constructor or a single constructor taking the message sink.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup.</typeparam>
    /// <seealso cref="Xunit.Fixture.Mvc.MvcFunctionalTestFixture{TStartup}" />
    public class MvcFunctionalTestClassFixture<TStartup> : MvcFunctionalTestFixture<TStartup>
        where TStartup : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MvcFunctionalTestClassFixture{TStartup}"/> class.
        /// </summary>
        /// <param name="sink">The message sink.</param>
        public MvcFunctionalTestClassFixture(IMessageSink sink) : base(null, sink)
        {
        }
    }
}