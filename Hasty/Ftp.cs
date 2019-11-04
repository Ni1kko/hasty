using FluentFTP;
using System; 
using System.Net; 
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    class Ftp {
        public static bool Cancel { get; set; } = false;
        public static async Task<bool> RequestFile(Repo repo, string file, string savePath, long fileLength, Action<FtpProgress> progress = null) {
            try {
                if (Cancel)
                    return false;

                bool isGood = IPAddress.TryParse(repo.FtpAddress, out IPAddress ipAddr);
                if (!isGood)
                    throw new Exception("Invalid FTP connection IP provided");


                FtpClient client = new FtpClient(repo.FtpAddress, 6969, "anonymous", "");

                await client.ConnectAsync();

                await client.DownloadFileAsync(savePath, file, FtpLocalExists.Overwrite, FtpVerify.None, new Progress<FtpProgress>(async x =>
                {
                    progress(x);

                    if (Cancel) {
                        await client.DisconnectAsync();
                    }
                }));


            } catch (Exception ex) {
                MessageBox.Show("File download failed: " + ex, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
