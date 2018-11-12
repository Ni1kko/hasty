using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public static async Task<Tuple<DynatreeItem, Exception>> ReadRepo(string url) {
            try {

                string json = await _client.DownloadStringTaskAsync(new Uri(url));
                DynatreeItem fileTree = DynatreeItem.JsonToDynatree(json);

                return Tuple.Create<DynatreeItem, Exception>(fileTree, null);
            } catch (Exception ex) {
                return Tuple.Create<DynatreeItem, Exception>(null, ex);
            }
        }

        public static async Task<bool> DownloadFile(string url, string filePath, Action<double> progressCallback = null) {
            try {
                WebClient wc = new WebClient();

                if (progressCallback != null) {
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler((object sender, DownloadProgressChangedEventArgs e) => {
                        double bytesIn = double.Parse(e.BytesReceived.ToString());
                        double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                        double percentage = bytesIn / totalBytes * 100;

                        progressCallback(percentage);
                    });
                }


                string data = await wc.DownloadStringTaskAsync(new Uri(url));

                wc.Dispose();
                return true;
                
            } catch (Exception ex) {
                MessageBox.Show("An error occured while downloading file: " + url + "\nError: " + ex, "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return false;
            }
        }

    }
}
