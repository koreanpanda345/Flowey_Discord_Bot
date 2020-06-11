using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
namespace Flowey.Bot
{
    class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";
        private const string configPath = configFolder + "/" + configFile;
        public static BotConfig Bot;
        static Config()
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);
            if (!File.Exists(configPath))
            {
                Bot = new BotConfig();
                string json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                Bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }
    public struct BotConfig
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string AirtableApi { get; set; }
        public string AirtableBaseApi { get; set; }
    }
}
