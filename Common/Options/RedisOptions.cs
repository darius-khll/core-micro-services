namespace Common.Options
{
    public class RedisOptions : ISettingOptions
    {
        public string host { get; } = "localhost:6379";
        public string name { get; }
        public int port { get; } = 6379;

        public static string GetConfigName => "redis";
    }
}
