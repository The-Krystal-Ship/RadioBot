using Microsoft.Extensions.DependencyInjection;

using RadioBot.Managers;
using RadioBot.Services;

namespace RadioBot.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddSingleton<EventManager>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<RadioService>();

            return services;
        }
    }
}
