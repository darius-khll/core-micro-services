namespace Common.Implementations
{
    public class MongoOptions : ISettingOptions
    {
        public string host { get; } = "localhost:27017";
        public string name { get; }
        public int port { get; } = 27017;

        public static string GetConfigName => "mongo";
    }
}
