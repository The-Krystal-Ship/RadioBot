using Discord.Commands;

using TheKrystalShip.DependencyInjection;
using TheKrystalShip.RadioBot.Core.Services;

namespace TheKrystalShip.RadioBot.Core.Modules
{
    public class Module : ModuleBase<SocketCommandContext>
    {
        public IRadioService Service { get; private set; }

        public Module()
        {
            Service = Container.Get<IRadioService>();
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);

            Service.SetContext(Context);
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);

        }
    }
}
