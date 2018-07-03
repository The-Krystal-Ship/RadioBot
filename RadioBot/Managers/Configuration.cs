using Microsoft.Extensions.Configuration;

using System.IO;

namespace RadioBot.Managers
{
    public static class Configuration
    {
        private static IConfiguration _config;

        public static IConfiguration Get
        {
            get
            {
                if (_config != null)
                    return _config;

                _config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(Path.Combine("Properties", "settings.json"), optional: false, reloadOnChange: true)
                    .Build();

                return _config;
            }
        }
    }
}
