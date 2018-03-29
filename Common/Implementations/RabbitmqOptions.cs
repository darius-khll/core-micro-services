namespace Common.Implementations
{
    public class RabbitmqOptions : ISettingOptions
    {
        public string host { get; } = "localhost:5672";
        public string name { get; }
        public int port { get; } = 5672;

        public string user { get; }
        public string password { get; }

        public static string GetConfigName => "rabbitmq";
    }
}
