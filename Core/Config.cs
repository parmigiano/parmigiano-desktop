using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Core
{
    public class AppConfig
    {
        [Newtonsoft.Json.JsonProperty("TYPE_RELEASE")]
        public string TYPE_RELEASE { get; } = "dev";

        [Newtonsoft.Json.JsonProperty("APP_NAME")]
        public string APP_NAME { get; } = "Parmigiano";

        [Newtonsoft.Json.JsonProperty("APP_FOLDER")]
        public string APP_FOLDER_PATH { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parmigiano");

        [Newtonsoft.Json.JsonProperty("CONFIG_USER_PATH")]
        public string CONFIG_USER_PATH { get; }

        public string SERVER_ADDR
        {
            get
            {
                if (TYPE_RELEASE == "dev")
                {
                    return "http://localhost:8080/api/v1";
                } else
                {
                    return "http://77.222.54.211/api/v1";
                }
            }
        }

        public AppConfig()
        {
            CONFIG_USER_PATH = Path.Combine(APP_FOLDER_PATH, "user.conf");
        }
    }

    public static class Config
    {
        public static AppConfig Current { get; } = new AppConfig();
    } //  Config.Current.AppName;
}
