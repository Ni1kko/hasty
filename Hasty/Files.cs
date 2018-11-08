﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hasty {
    class Files {
        private static string _appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";
        private static string _folderName = "Hasty";
        private static string _settingsFolder = Path.Combine(_appData, _folderName);
        private static string _settingsFile = Path.Combine(_settingsFolder, "settings.json");


        public static void Init() {
            try {
                if (!Directory.Exists(_settingsFolder))
                    Directory.CreateDirectory(_settingsFolder);

                if (!File.Exists(_settingsFile))
                    SetSettings(DefaultSettings());
                    
            }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString(), "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Settings DefaultSettings() {
            Settings s = new Settings();
            s.Repos = new List<Repo>();

            return s;
        }

        public static void SetSettings(Settings settings) {
            try {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_settingsFile, json);
            }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString(), "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Settings GetSettings() {
            try { 
                string json = File.ReadAllText(_settingsFile);
                return JsonConvert.DeserializeObject<Settings>(json);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return DefaultSettings();
        }

        public static void UpdateRepos(List<Repo> repos) {
            Settings s = GetSettings();
            s.Repos = repos;
            SetSettings(s);
        }

        public static Repo Update(Repo old, Repo newer) {
            Repo ret = new Repo();
            if (newer.Name == null) {
                ret.Name = old.Name;
            } else {
                ret.Name = newer.Name;
            }

            if (newer.Url == null) {
                ret.Url = old.Url;
            } else {
                ret.Url = newer.Url;
            }

            if (newer.RemoteFolder == null) {
                ret.RemoteFolder = old.RemoteFolder;
            } else {
                ret.RemoteFolder = newer.RemoteFolder;
            }

            ret.LastCheck = Misc.UnixTime;
            ret.LastUpdate = Misc.UnixTime;

            if (newer.Folder == null) {
                ret.Folder = old.Folder;
            } else {
                ret.Folder = newer.Folder;
            }

            return ret;
        }
    }
}