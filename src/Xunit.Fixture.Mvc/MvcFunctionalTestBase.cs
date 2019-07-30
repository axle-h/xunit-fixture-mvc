using Xunit.Abstractions;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// Base for test classes wishing to share an MVC functional test fixture across tests.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup.</typeparam>
    /// <seealso cref="MvcFunctionalTestClassFixture{TStartup}" />
    public abstract class MvcFunctionalTestBase<TStartup> : IClassFixture<MvcFunctionalTestClassFixture<TStartup>>
        where TStartup : class
    {
        private readonly IMvcFunctionalTestFixture _fixture;
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcFunctionalTestBase{TStartup}"/> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="output">The output.</param>
        protected MvcFunctionalTestBase(MvcFunctionalTestClassFixture<TStartup> fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        /// <summary>
        /// Retrieves the class fixture.
        /// </summary>
        /// <returns></returns>
        protected IMvcFunctionalTestFixture GivenClassFixture() => GivenBootstrap(_fixture
            .HavingTestOutput(_output)
            .HavingSetup(f => GivenSetup(f))
            .Build());

        /// <summary>
        /// Builds a new, isolated fixture.
        /// </summary>
        /// <returns></returns>
        protected IMvcFunctionalTestFixture GivenIsolatedFixture() =>
            GivenSetup(new MvcFunctionalTestFixture<TStartup>(_output));

        /// <summary>
        /// Adds fixture actions to be run for every test.
        /// This function is called on the fixture after it is built so cannot do any setup actions.
        /// For setup actions you should override <see cref="GivenSetup"/>.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        protected virtual IMvcFunctionalTestFixture GivenBootstrap(IMvcFunctionalTestFixture fixture) => fixture;

        /// <summary>
        /// Sets up the specified fixture.
        /// This is run only once for the class fixture i.e. across all tests in this class that consume <see cref="GivenClassFixture"/>.
        /// For actions that need to be run for every test you should override <see cref="GivenBootstrap"/>.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        protected virtual IMvcFunctionalTestFixture GivenSetup(IMvcFunctionalTestFixture fixture) => fixture;
    }
}