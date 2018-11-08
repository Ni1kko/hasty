using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasty {
    class Repo {
        public string Name;
        public string Url;
        public string RemoteFolder;
        public int LastCheck;
        public int LastUpdate;
        public string Folder;
    }

    class Settings {
        public List<Repo> Repos;
    }

}
