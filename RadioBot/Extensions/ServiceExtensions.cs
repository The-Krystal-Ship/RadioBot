using Microsoft.Extensions.DependencyInjection;

using TheKrystalShip.RadioBot.Managers;
using TheKrystalShip.RadioBot.Services;

namespace TheKrystalShip.RadioBot.Extensions
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
            services.AddSingleton<IStreamService, StreamService>();
            services.AddSingleton<IDownloadService, DownloadService>();
            services.AddSingleton<IEncodeService, EncodeService>();

            return services;
        }
    }
}
