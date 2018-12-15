using Microsoft.Extensions.Configuration;

using System.IO;

namespace TheKrystalShip.RadioBot.Tools
{
    public static class Configuration
    {
        private static readonly string _settingsFile = Path.Combine("Properties", "settings.json");
        private static readonly string _secretsFile = Path.Combine("Properties", "secrets.json");
        private static readonly IConfiguration _config;

        static Configuration()
        {
            Directory.CreateDirectory("Properties");

            if (!File.Exists(_settingsFile))
                File.Create(_settingsFile);

            if (!File.Exists(_secretsFile))
                File.Create(_secretsFile);

            _config = new ConfigurationBuilder()
                    .AddJsonFile(_settingsFile, optional: false, reloadOnChange: true)
                    .AddJsonFile(_secretsFile, optional: true, reloadOnChange: true)
                    .Build();
        }

        public static string Get(string key)
        {
            return _config[key];
        }
    }
}
