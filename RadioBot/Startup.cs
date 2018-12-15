using System.Threading.Tasks;

using TheKrystalShip.DependencyInjection;
using TheKrystalShip.RadioBot.Core;
using TheKrystalShip.RadioBot.Core.Commands;
using TheKrystalShip.RadioBot.Core.Services;
using TheKrystalShip.RadioBot.Tools;

namespace TheKrystalShip.RadioBot
{
    public class Startup
    {
        public Startup()
        {

        }

        public Startup ConfigureCommands()
        {
            Container.Add<CommandHandler>();
            Container.Get<CommandHandler>();

            return this;
        }

        public Startup ConfigureServices()
        {
            Container.Add<AudioPlayer>();
            Container.Add<IRadioService, RadioService>();

            return this;
        }

        public Startup ConfigureClient()
        {
            Bot client = new Bot();

            client.InitAsync(Configuration.Get("Bot:Token")).Wait();

            return this;
        }

        public async Task InitAsync()
        {
            await Task.Delay(-1);
        }
    }
}
