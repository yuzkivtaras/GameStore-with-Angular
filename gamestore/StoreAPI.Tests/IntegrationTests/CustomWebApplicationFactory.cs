using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace StoreAPI.Tests.IntegrationTests
{
    public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = builder.Build();

            host.StartAsync().GetAwaiter().GetResult();

            return host;
        }
    }
}
