using Microsoft.Extensions.DependencyInjection;

namespace TheKrystalShip.RadioBot
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
            services.AddSingleton<IRadioService, RadioService>();
            services.AddSingleton<AudioPlayer>();

            return services;
        }

        public static IServiceCollection AddTools(this IServiceCollection services)
        {
            services.AddSingleton<Tools>();

            return services;
        }
    }
}
