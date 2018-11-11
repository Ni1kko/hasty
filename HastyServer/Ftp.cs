using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics;

namespace HastyServer {
    class Ftp {

        private static string _zipFile = "FileZilla.zip";
        private static string _zillaFolder = "FileZilla";

        public static bool Init() {
            if (!File.Exists(_zillaFolder + "/FileZilla Server.exe")) {
                try {
                    using (WebClient wc = new WebClient()) {
                        wc.DownloadFile("https://eelis.me/FileZilla.zip", _zipFile);
                    }
                } catch(Exception ex) {
                    Console.WriteLine("Downloading FTP server failed: " + ex);
                    return false;
                }

                if (!Directory.Exists(_zillaFolder))
                    Directory.CreateDirectory(_zillaFolder);

                try {
                    ZipFile.ExtractToDirectory(_zipFile, _zillaFolder);
                    File.Delete(_zipFile);
                }
                catch(Exception ex) {
                    Console.WriteLine("Failed to extact FileZilla: " + ex);
                    return false;
                }
            }
            return true;
        }

        public static bool Start() {
            Settings settings = Settings.ReadSettings();

            string modFolder = settings.ModFolder;

            string path = modFolder;
            if (!Path.IsPathRooted(modFolder)) {
                path = Environment.CurrentDirectory + "/" + modFolder;
            }

            try {
                string zillaSettings = File.ReadAllText(_zillaFolder + "/FileZilla Server.xml.bak");

                zillaSettings = zillaSettings.Replace("$serverfolder$", path);
                File.WriteAllText(_zillaFolder + "/FileZilla Server.xml", zillaSettings);

                Process p = Process.Start(Environment.CurrentDirectory + "/" + _zillaFolder + "/FileZilla Server.exe", "/install");

                if (!p.WaitForExit(5000))
                    throw new Exception("FileZilla server failed to install...");

                p = Process.Start(Environment.CurrentDirectory + "/" + _zillaFolder + "/FileZilla Server.exe", "/stop");

                if (!p.WaitForExit(5000))
                    throw new Exception("FileZilla server failed to stop...");

                Process.Start(Environment.CurrentDirectory + "/" + _zillaFolder + "/FileZilla Server.exe", "/start");
            }
            catch(Exception ex) {
                Console.WriteLine("Failed to configure filezilla: " + ex);
                return false;
            }

            return true;
        }
    }
}
