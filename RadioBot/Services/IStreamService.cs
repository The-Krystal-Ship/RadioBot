using System.Diagnostics;

namespace TheKrystalShip.RadioBot.Services
{
    public interface IStreamService
    {
        Process CreateStream(string query);
    }
}
