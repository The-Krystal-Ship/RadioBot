using Discord.Commands;

namespace TheKrystalShip.RadioBot
{
    public class Module : ModuleBase<SocketCommandContext>
    {
        public Tools Tools { get; private set; }

        public Module(Tools tools)
        {
            Tools = tools;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);

        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);

        }
    }
}
