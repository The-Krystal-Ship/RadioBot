using Microsoft.Extensions.Configuration;

namespace TheKrystalShip.RadioBot
{
    public static class ConfigurationExtensions
    {
        public static string GetToken(this IConfiguration config)
        {
            return config["Bot:Token"];
        }
    }
}
