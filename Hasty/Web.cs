using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hasty {
    class Web {

        private static WebClient _client = new WebClient();

        public static async Task<Tuple<Repo, Exception>> ReadUrlAsync(string url) {
            try {
                string json = await _client.DownloadStringTaskAsync(new Uri(url));
                Repo repo = JsonConvert.DeserializeObject<Repo>(json);

                return Tuple.Create<Repo, Exception>(repo, null);
            } catch(Exception ex) {
                return Tuple.Create<Repo, Exception>(null, ex);
            }
        } 

    }
}
