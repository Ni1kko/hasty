using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastyServer {
    class FileStorage {
        Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
        Settings _settings;


        public FileStorage() {
            _settings = Settings.ReadSettings();
        }

        public Stream ReadFile(string file) {
            if (!File.Exists(file))
                return null;

            FileInfo fi = new FileInfo(file);
            if (_cache.ContainsKey(fi.FullName)) {
                MemoryStream ms = new MemoryStream(_cache[fi.FullName]);
                return ms;
            } else {
                FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                return fs;
            }
        }
    }
}
