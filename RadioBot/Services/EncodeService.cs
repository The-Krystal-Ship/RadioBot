using TheKrystalShip.Configuration;
using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Services
{
    public class EncodeService : IEncodeService
    {
        private readonly ILogger<EncodeService> _logger;
        private readonly string _program;
        private readonly string _launchArgs;
        private readonly string _inputArgs;
        private readonly string _outputArgs;

        public EncodeService(ILogger<EncodeService> logger)
        {
            _logger = logger;

            _program = Settings.Instance["Encode:Executable"];
            _launchArgs = Settings.Instance["Encode:Args"];
            _inputArgs = Settings.Instance["Encode:Input"];
            _outputArgs = Settings.Instance["Encode:Output"];
        }

        public override string ToString()
        {
            string toReturn = $"{_program} {_launchArgs} {_inputArgs} {_outputArgs}";

            _logger.LogInformation(toReturn);

            return toReturn;
        }
    }
}
