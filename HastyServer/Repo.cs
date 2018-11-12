using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HastyServer {
    class Repo {
        public string Name;
        public string RemoteFolder;
        public string FtpAddress;
        public string FolderName;
        public int LastCheck;
        public int LastUpdate;
        public string Folder;

        public static void WriteDefaults() { 
            string ip = "Unknown";
            try {
                using (WebClient wc = new WebClient()) {
                    ip = wc.DownloadString("https://eelis.me/ip.php");
                }
            } catch (Exception ex) {
                Console.WriteLine("Failed to obtain server IP, error: " + ex);
            }


            Repo r = new Repo();
            r.Name = "New Repository";
            r.RemoteFolder = "http://" + ip + "/mod";
            r.FtpAddress = ip;
            r.FolderName = "@your-mod";

            string json = JsonConvert.SerializeObject(r);
            File.WriteAllText("repo.json", json);
        }
    }

}
