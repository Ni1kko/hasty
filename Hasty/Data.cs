using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasty {
    class Repo {
        public string Name;
        public string Url;
        public string RemoteFolder;
        public string SocketConnection;
        public string FolderName;
        public int LastCheck;
        public int LastUpdate;
        public int BufferSize;
        public string Folder;
    }

    class Settings {
        public List<Repo> Repos;
    }

    class DynatreeItem {
        public string title { get; set; }
        public bool isFolder { get; set; }
        public string hash { get; set; }
        public long fileSize;
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
            } else {
                isFolder = false;
            }
            hash = title.Replace(" ", "").ToLower();
        }

        public DynatreeItem() { }

        public string DynatreeToJson() {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }

        public static DynatreeItem JsonToDynatree(string json) {
            return JsonConvert.DeserializeObject<DynatreeItem>(json);
        }
    }
}
