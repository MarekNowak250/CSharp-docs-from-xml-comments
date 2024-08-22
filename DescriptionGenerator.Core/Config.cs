using Newtonsoft.Json;

namespace DescriptionGenerator.Core
{
    public class Config
    {
        private const string PATH = "./config.json";
        public bool IncludeNested { get; set; } = true;
        public bool NamespaceLikeStructure { get; set; } = false;
        public bool IncludePropertiesSummary { get; set; } = true;
        public bool IncludeContainersSummary { get; set; } = true;
        public bool GenerateLinks { get; set; } = true;

        public static Config LoadConfig(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = PATH;

            if (!File.Exists(path))
                return new Config();

            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path)) ?? throw new Exception("Invalid config file!");
        }

        public void SaveConfig(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = PATH;

            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }
    }
}