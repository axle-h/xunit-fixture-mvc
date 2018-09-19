using System;
using System.Threading.Tasks;

namespace Xunit.Fixture.Mvc
{
    internal class SimpleTestServerBootstrap : ITestServerBootstrap
    {
        private readonly IServiceProvider _provider;
        private readonly Func<IServiceProvider, Task> _action;

        public SimpleTestServerBootstrap(IServiceProvider provider, Func<IServiceProvider, Task> action)
        {
            _provider = provider;
            _action = action;
        }

        public Task BootstrapAsync() => _action(_provider);
    }
}