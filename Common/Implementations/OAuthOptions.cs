using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Implementations
{
    public class OAuthOptions : ISettingOptions
    {
        public string host { get; } = "localhost:8183";
        public string name { get; }
        public int port { get; } = 8183;

        public static string GetConfigName => "oauth";
    }
}
