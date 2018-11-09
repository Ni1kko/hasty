using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HastyServer
{
    class Settings
    {
        public int CahcingMegs;
        public string ModFolder;
        public int NetworkSpeed;
        public int MaxSpeed;
        public int MinSpeed;

        public static Settings ReadSettings()
        {
            if (!File.Exists("setting.json"))
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
            s.CahcingMegs = 0;
            s.ModFolder = "mod";
            s.NetworkSpeed = 0;
            s.MaxSpeed = 0;
            s.MinSpeed = 0;
            return s;
        }
    }
}
