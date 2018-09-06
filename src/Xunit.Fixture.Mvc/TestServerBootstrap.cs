using System.Threading.Tasks;

namespace Xunit.Fixture.Mvc
{
    /// <summary>
    /// A simple service for bootstrapping the test server.
    /// </summary>
    public interface ITestServerBootstrap
    {
        /// <summary>
        /// Executes the bootstrap method.
        /// </summary>
        /// <returns></returns>
        Task BootstrapAsync();
    }
}