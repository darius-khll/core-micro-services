namespace Common.Options
{
    public interface ISettingOptions
    {
        string host { get; }
        string name { get; }
        int port { get; }

        //public static string GetConfigName => "something";   it is important to get config
    }
}
