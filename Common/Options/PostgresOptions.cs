using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Options
{
    public class PostgresOptions : ISettingOptions
    {
        public string host { get; set; }
        public string name { get; set; }
        public int port { get; set; }

        public static string GetConfigName => "postgres";
    }
}
