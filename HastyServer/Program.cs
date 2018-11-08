using NHttp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace HastyServer {

    class DynatreeItem {
        public string title { get; set; }
        public bool isFolder { get; set; }
        public string hash { get; set; }
        public List<DynatreeItem> children;
        public int totalFiles;

        public DynatreeItem(FileSystemInfo fsi) {
            title = fsi.Name;
            children = new List<DynatreeItem>();

            if (fsi.Attributes == FileAttributes.Directory) {
                isFolder = true;
                foreach (FileSystemInfo f in (fsi as DirectoryInfo).GetFileSystemInfos()) {
                    children.Add(new DynatreeItem(f));
                }
                hash = "";
            } else {
                isFolder = false;
                hash = CheckSum(fsi.FullName);
            }
            
        }

        public string DynatreeToJson() {
            return JsonConvert.SerializeObject(this);
        }

        public string CheckSum(string filePath) {
            using (SHA512 SHA512 = SHA512.Create()) {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return Convert.ToBase64String(SHA512.ComputeHash(fileStream));
            }
        }

        public static DynatreeItem JsonToDynatree(string json) {
            return JsonConvert.DeserializeObject<DynatreeItem>(json);
        }
    }

    class Program {

        static void Main(string[] args) {

            if (!File.Exists("repo.json")) {
                Console.WriteLine("Unable to start Hasty server, repo.json is missing");
                Console.ReadLine();
                return;
            }

            if (!Directory.Exists("mod")) {
                Console.WriteLine("Unable to start Hasty server, mod folder is not configured/does not exist");
                Console.ReadLine();
                return;
            }

            

            HttpServer server = new HttpServer();
            server.RequestReceived += (s, e) => {
                string path = e.Request.Path;
                Console.WriteLine("Request for " + path);

                using (var writer = new StreamWriter(e.Response.OutputStream)) {
                    
                    if (path == "/" || path == "/repo.json") {
                        writer.Write(File.ReadAllText("repo.json"));
                    } else if (path == "/mod") {

                        DynatreeItem di = new DynatreeItem(new DirectoryInfo("mod"));

                        int files = Directory.GetFiles("mod", "*", SearchOption.AllDirectories).Length;
                        di.totalFiles = files;
                        string result = di.DynatreeToJson();
                        writer.Write(result);

                    } else if (path.StartsWith("/mod")) {
                        string file = path.Substring(1);

                        Console.WriteLine("File request: " + file);

                        byte[] data = Encoding.UTF8.GetBytes("File not found...");

                        if (File.Exists(file) && !path.Contains("..")) {
                            data = File.ReadAllBytes(file);
                        }

                        writer.Write(new Ascii85().Encode(data));

                    } else {

                        writer.Write("404 not found");
                    }
                }
            };

            server.EndPoint = new IPEndPoint(IPAddress.Loopback, 80);

            server.Start();
            

            Console.WriteLine("Server running...");
            Console.ReadLine();
        }
    }
}
