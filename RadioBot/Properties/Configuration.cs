using Microsoft.Extensions.Configuration;

using System;
using System.IO;

namespace TheKrystalShip.RadioBot.Properties
{
    public static class Configuration
    {
        private static IConfiguration _config;
        private static readonly string _configFolder = Path.Combine("Properties");
        private static readonly string _configFile = Path.Combine("settings.json");
        private static readonly string _configPath = Path.Combine(_configFolder, _configFile);

        public static IConfiguration Instance
        {
            get
            {
                if (_config != null)
                    return _config;

                if (!Directory.Exists(_configFolder))
                    throw new DirectoryNotFoundException();

                if (!File.Exists(_configPath))
                    throw new FileNotFoundException();

                return _config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(_configPath, optional: false, reloadOnChange: true)
                        .Build();
            }
        }

        public static string Get(params string[] index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            int limit = index.Length - 1;
            IConfigurationSection section = null;

            for (int i = 0; i < limit; i++)
            {
                section = Instance.GetSection(index[i]);
            }

            return section[index[limit]];
        }
    }
}
