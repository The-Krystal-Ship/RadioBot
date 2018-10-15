using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot
{
    public static class TaskExtensions
    {
        public static async Task DelayIndefinetly(this Task task) => await Task.Delay(-1);
    }
}
