using Microsoft.Extensions.DependencyInjection;
using ZeyjaFramework.Interfaces;

namespace ZeyjaFramework
{
    public class Startup
    {
        /// <summary>
        /// Configures the service providers.
        /// </summary>
        /// 
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(IDatabase), new Database()));
        }
    }
}
