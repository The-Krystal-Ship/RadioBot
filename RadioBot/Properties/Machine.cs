using System;

namespace TheKrystalShip.RadioBot.Properties
{
    public static class Machine
    {
        public static string User {  get => Environment.GetEnvironmentVariable("USERNAME") ?? Environment.GetEnvironmentVariable("USER"); }
        public static string Domain { get => Environment.GetEnvironmentVariable("USERDOMAIN") ?? Environment.GetEnvironmentVariable("HOSTNAME"); }
    }
}
