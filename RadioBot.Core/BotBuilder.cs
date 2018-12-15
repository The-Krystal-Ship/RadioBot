namespace RadioBot.Core
{
    public static class BotBuilder
    {
        public static T UseStartup<T>() where T : new()
        {
            return new T();
        }
    }
}
