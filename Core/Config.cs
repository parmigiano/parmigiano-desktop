using System;
using System.IO;

namespace Parmigiano.Core
{
    public class AppConfig
    {
        [Newtonsoft.Json.JsonProperty("TYPE_RELEASE")]
        public string TYPE_RELEASE { get; } =
        #if DEBUG
            "dev";
        #else
            "prod";
        #endif

        [Newtonsoft.Json.JsonProperty("APP_NAME")]
        public string APP_NAME { get; } = "Parmigiano";

        [Newtonsoft.Json.JsonProperty("BROKER_APP_NAME")]
        public string BROKER_APP_NAME { get; } = "ParmigianoChatBroker.exe";

        [Newtonsoft.Json.JsonProperty("BROKER_PROCESS_NAME")]
        public string BROKER_PROCESS_NAME { get; } = "ParmigianoChatNotify";

        [Newtonsoft.Json.JsonProperty("APP_FOLDER")]
        public string APP_FOLDER_PATH { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Parmigiano");

        [Newtonsoft.Json.JsonProperty("CONFIG_USER_PATH")]
        public string CONFIG_USER_PATH { get; }

        [Newtonsoft.Json.JsonProperty("LOGS_PATH")]
        public string LOGS_PATH { get; }

        [Newtonsoft.Json.JsonProperty("HTTP_SERVER_ADDR")]
        public string HTTP_SERVER_ADDR { get; } =
        #if DEBUG
            "http://localhost:8080/api/v1/";
        #else
            "https://parmigianochat.ru/api/v1/";
        #endif

        [Newtonsoft.Json.JsonProperty("WSOCKET_SERVER_ADDR")]
        public string WSOCKET_SERVER_ADDR { get; } =
        #if DEBUG
            "ws://localhost:8080";
        #else
            "wss://parmigianochat.ru";
        #endif

        [Newtonsoft.Json.JsonProperty("TCP_SERVER_ADDR")]
        public string TCP_SERVER_ADDR { get; set; } = "77.222.54.211"; // 77.222.54.211 62.16.41.188

        [Newtonsoft.Json.JsonProperty("TCP_SERVER_PORT")]
        public short TCP_SERVER_PORT = 5462;

        public AppConfig()
        {
            this.CONFIG_USER_PATH = Path.Combine(APP_FOLDER_PATH, "user.config");
            this.LOGS_PATH = Path.Combine(APP_FOLDER_PATH, "Logs");
        }
    }

    public static class Config
    {
        public static AppConfig Current { get; } = new AppConfig();
    } //  Config.Current.AppName;
}
