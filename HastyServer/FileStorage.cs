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

            int megs = _settings.CahcingMegs;
            if (megs > 0) {
                if (Directory.Exists(_settings.ModFolder)) {
                    string[] files = Directory.GetFiles(_settings.ModFolder, "*", SearchOption.AllDirectories);

                    int totalBytesUsed = 0;
                    foreach (string s in files) {
                        byte[] buf = File.ReadAllBytes(s);


                        // techically could fit more files in, but cba to implement more logic
                        if (buf.Length + totalBytesUsed > megs * 1000000)
                            break;

                        totalBytesUsed += buf.Length;
                        FileInfo fi = new FileInfo(s);
                        _cache.Add(fi.FullName, buf);
                    }
                }
            }
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
