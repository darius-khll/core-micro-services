namespace Common.Options
{
    public class RabbitmqOptions : ISettingOptions
    {
        public string host { get; set; }
        public string name { get; set; }
        public int port { get; set; }

        public string user { get; set; }
        public string password { get; set; }

        public static string GetConfigName => "rabbitmq";
    }
}
