using Newtonsoft.Json;

namespace DescriptionGenerator
{
    internal class Config
    {
        private const string PATH = "./config.json";
        public string OutputPath { get; set; }

        public static Config LoadConfig()
        {
            if (!File.Exists(PATH))
                throw new Exception("No config file!");

            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(PATH)) ?? throw new Exception("Invalid config file!");
        }
    }
}