using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Linq;

namespace HastyServer {

    class TreeCache {
        public static Dictionary<string, string> hashes = new Dictionary<string, string>();
        public static int totalFiles = 0;
        public static int processedFiles = 0;
    }

    class DynatreeItem {
        public string title { get; set; }
        public bool isFolder { get; set; }
        public string hash { get; set; }
        public long fileSize;
        public List<DynatreeItem> children;
        public int totalFiles;

        public DynatreeItem(FileSystemInfo fsi, bool refresh = false) {
            try {
                if (refresh) {
                    TreeCache.hashes.Clear();
                    TreeCache.totalFiles = 0;
                }

                if (TreeCache.totalFiles == 0) {
                    try {
                        //Console.WriteLine(fsi.FullName);
                        int allFiles = Directory.GetFiles(fsi.FullName, "*", SearchOption.AllDirectories).Length;
                        TreeCache.totalFiles = allFiles;
                    }
                    catch {
                    }
                }


                TreeCache.processedFiles++;

                Console.Write($"\rHashed files: {TreeCache.processedFiles}/{TreeCache.totalFiles}");

                title = fsi.Name;
                children = new List<DynatreeItem>();

                if (fsi.Attributes == FileAttributes.Directory) {
                    isFolder = true;
                    foreach (FileSystemInfo f in (fsi as DirectoryInfo).GetFileSystemInfos()) {
                        children.Add(new DynatreeItem(f, refresh));
                    }
                    hash = "";
                } else {

                    isFolder = false;
                    fileSize = new FileInfo(fsi.FullName).Length;
                    if (TreeCache.hashes.ContainsKey(fsi.FullName)) {
                        hash = TreeCache.hashes[fsi.FullName];
                    } else {
                        hash = CheckSum(fsi.FullName);
                        TreeCache.hashes[fsi.FullName] = hash;
                    }

                }
            }
            catch(Exception ex) {
                Console.WriteLine("File tree exception: " + ex.Message);
            }
            
        }

        public string DynatreeToJson() {
            return JsonConvert.SerializeObject(this);
        }

        public string CheckSum(string filePath) {
            using (var inputStream = File.Open(filePath, FileMode.Open)) {
                var md5 = MD5.Create();
                return Convert.ToBase64String(md5.ComputeHash(inputStream));
            }
        }

        public static DynatreeItem JsonToDynatree(string json) {
            return JsonConvert.DeserializeObject<DynatreeItem>(json);
        }
    }

    class Program {

        static Thread updateThread = null;
        static DynatreeItem di;
        static string modFolder = "mod";

        private static void OnFileChange(object source, FileSystemEventArgs e) {
            Console.WriteLine("File: " + e.FullPath + " was changed: " + e.ChangeType);

            try {
                bool sleepAgain = true;
                if (updateThread != null && updateThread.IsAlive) {
                    sleepAgain = true;
                    return;
                }


                updateThread = new Thread(() => {
                    try {
                        while (sleepAgain) {
                            sleepAgain = false;
                            Thread.Sleep(15000);
                        }

                        sleepAgain = true;
                        Console.WriteLine("Generating new file hashes..");
                        di = new DynatreeItem(new DirectoryInfo(modFolder), true);

                        Console.WriteLine("\nNew hashes generated...");
                    }
                    catch { }
                });
                updateThread.Start();
            } catch (Exception ex) {
                Console.WriteLine("File changed exception: " + ex);
            }
        }

        static void Main(string[] args) {

            Settings settings = Settings.ReadSettings();
            modFolder = settings.ModFolder;


            if (!File.Exists("repo.json")) {
                Console.WriteLine("Unable to start Hasty server, repo.json is missing");
                Console.ReadLine();
                return;
            }

            if (!Directory.Exists(modFolder)) {
                Console.WriteLine("Unable to start Hasty server, mod folder is not configured/does not exist");
                Console.ReadLine();
                return;
            }

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = modFolder;
            watcher.IncludeSubdirectories = true;

            watcher.Changed += new FileSystemEventHandler(OnFileChange);
            watcher.Created += new FileSystemEventHandler(OnFileChange);

            watcher.EnableRaisingEvents = true;
            

            Console.WriteLine("Generating file hashes...");
            
            DirectoryInfo dirInfo = new DirectoryInfo(modFolder);
            di = new DynatreeItem(dirInfo);

            Console.WriteLine("\nHashes generated...");

            //new Thread(TcpCommunication.StartListener).Start();
            if (!Ftp.Init())
                Console.WriteLine("Failed to init FTP");

            if (!Ftp.Start())
                Console.WriteLine("Failed to start FTP");

            HttpServer server = new HttpServer();
            server.RequestReceived += (s, e) => {
                string path = e.Request.Path;
                Console.WriteLine("Request for " + path);

                using (var writer = new StreamWriter(e.Response.OutputStream)) {
                    
                    if (path == "/" || path == "/repo.json") {
                        JObject json = JObject.Parse(File.ReadAllText("repo.json"));
                        json.Add("BufferSize", TcpCommunication.BufferSize);
                        writer.Write(json.ToString());
                    } else if (path == "/mod") {

                        int files = Directory.GetFiles(modFolder, "*", SearchOption.AllDirectories).Length;
                        di.totalFiles = files;
                        string result = di.DynatreeToJson();
                        writer.Write(result);

                    } else {

                        writer.Write("404 not found");
                    }
                }
            };

            server.EndPoint = new IPEndPoint(IPAddress.Any, 8090);

            server.Start();
            

            Console.WriteLine("Server running...");
            Console.ReadLine();
        }
    }
}
