using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot
{
    public class Program
    {
        private static Bot _radioBot;

        public static async Task Main(string[] args)
        {
            await (_radioBot = new Bot())
                .InitAsync()
                .DelayIndefinetly();
        }
    }
}
