using Microsoft.Extensions.Configuration;

namespace TheKrystalShip.RadioBot.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetToken(this IConfiguration config)
        {
            return config.GetSection("Bot")["Token"];
        }
    }
}
