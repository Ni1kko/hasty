using System.IO; 
using Newtonsoft.Json;

namespace HastyServer
{
    class Settings
    {
        public string ModFolder;

        public static Settings ReadSettings()
        {
            if (!File.Exists("settings.json"))
            {
                SetSettings(DefaultSettings());
            }

            string json = File.ReadAllText("settings.json");
            Settings s = JsonConvert.DeserializeObject<Settings>(json);
            return s;
        }

        public static void SetSettings(Settings s)
        {
            string json = JsonConvert.SerializeObject(s);
            File.WriteAllText("settings.json", json);
        }

        public static Settings DefaultSettings()
        {
            Settings s = new Settings();
            s.ModFolder = "mod";
            return s;
        }
    }
}
