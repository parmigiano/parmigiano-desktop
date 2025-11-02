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

        [Newtonsoft.Json.JsonProperty("LOGS_PATH")]
        public string LOGS_PATH { get; }

        [Newtonsoft.Json.JsonProperty("HTTP_SERVER_ADDR")]
        public string HTTP_SERVER_ADDR
        {
            get
            {
                if (this.TYPE_RELEASE == "dev")
                {
                    return "http://localhost:8080/api/v1/";
                } else
                {
                    return "https://parmigianochat.ru/api/v1/";
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("WSOCKET_SERVER_ADDR")]
        public string WSOCKET_SERVER_ADDR
        {
            get
            {
                if (this.TYPE_RELEASE == "dev")
                {
                    return "ws://localhost:8080";
                }
                else
                {
                    return "wss://parmigianochat.ru";
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("TCP_SERVER_ADDR")]
        public string TCP_SERVER_ADDR
        {
            get
            {
                if (this.TYPE_RELEASE == "dev")
                {
                    return "localhost";
                } else
                {
                    return "77.222.54.211";
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("TCP_SERVER_PORT")]
        public short TCP_SERVER_PORT = 5462;

        public AppConfig()
        {
            this.CONFIG_USER_PATH = Path.Combine(APP_FOLDER_PATH, "user.config");
            this.LOGS_PATH = Path.Combine(APP_FOLDER_PATH, "Logs", "app.log");
        }
    }

    public static class Config
    {
        public static AppConfig Current { get; } = new AppConfig();
    } //  Config.Current.AppName;
}
