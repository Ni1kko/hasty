using System; 
using System.Net; 
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    class Http {

        public static async Task<bool> RequestFile(Repo repo, string file, string savePath, long fileLength, Action<long> progress = null) {

            try {
                string url = repo.FtpAddress;
                if (!url.StartsWith("http://"))
                    url = "http://" + url;

                url = url + "/mod/" + file;

                var wc = new WebClient();

                wc.DownloadProgressChanged += (s,e) => {
                    progress?.Invoke(e.ProgressPercentage);
                };
 
                wc.DownloadFileAsync(new Uri(url), savePath);

                await Task.Delay(1);

                wc.Dispose(); 

            } catch (Exception ex) {
                MessageBox.Show("File download failed: " + ex, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

    }
}
